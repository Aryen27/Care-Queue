using System.Security.Cryptography;
using System.Text;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Repositories.Interfaces;

namespace carequeue.CQ.API.Services.Internal
{
    public class OtpService
    {
        private readonly IOtpVerificationRepository _otpRepository;
        private readonly NotificationService _notificationService;
        private readonly ICustomerRepository _customerRepository;

        private const int MaxFailedAttempts = 3;
        private const int ExpirationMinutes = 10;

        public OtpService(
            IOtpVerificationRepository otpRepository,
            NotificationService notificationService,
            ICustomerRepository customerRepository)
        {
            _otpRepository = otpRepository;
            _notificationService = notificationService;
            _customerRepository = customerRepository;
        }

        public async Task<ServiceResult<string>> GenerateAndSendOtpAsync(int customerId, OtpPurpose purpose)
        {
            // 1. Generating a secure 6-digit num string
            string rawCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            // 2. DB stores hashed code
            string hashedCode = ComputeSha256Hash(rawCode);

            var otpVerification = new OtpVerification
            {
                CustomerId = customerId,
                OtpCodeHash = hashedCode,
                Purpose = purpose,
                ExpiresAt = DateTime.UtcNow.AddMinutes(ExpirationMinutes),
                CreatedAt = DateTime.UtcNow,
                Attempts = 0,
                UsedAt = null
            };

            await _otpRepository.AddAsync(otpVerification);
            await _otpRepository.SaveChangesAsync();

            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return ServiceResult<string>.Fail(
                    ErrorCodes.NotFound,
                    "Customer not found.");
            }

            var notification = await _notificationService.CreateNotificationAsync(
                new NotificationRequest
                {
                    HospitalId = 0,

                    CustomerId = customerId,

                    PatientId = Guid.Empty,

                    TemplateType = purpose switch
                    {
                        OtpPurpose.EmailVerification =>
                            NotificationTemplateType.EmailVerification,

                        OtpPurpose.PasswordReset =>
                            NotificationTemplateType.PasswordReset,

                        OtpPurpose.Login =>
                            NotificationTemplateType.Login,

                        OtpPurpose.Payment =>
                            NotificationTemplateType.Payment,

                        _  => throw new InvalidOperationException()
                    },

                    TemplateValues = new()
                    {
                        ["OTP"] = rawCode,
                        ["ExpiryMinutes"] = ExpirationMinutes.ToString()
                    }
                });

            if (!notification.Success)
            {
                return ServiceResult<string>.Fail(
                    notification.Error.Code,
                    notification.Error.Message!);
            }

            return ServiceResult<string>.Ok(rawCode);
        }

        public async Task<ServiceResult> VerifyOtpAsync(int customerId, OtpPurpose purpose, string submittedRawCode)
        {
            if (string.IsNullOrWhiteSpace(submittedRawCode))
            {
                return ServiceResult.Fail(ErrorCodes.Validation, "Verification code cannot be empty.");
            }

            var latestOtp = await _otpRepository.GetLatestOtpAsync(customerId, purpose);

            if (latestOtp == null)
            {
                return ServiceResult.Fail(ErrorCodes.NotFound, "No verification token found for this account.");
            }

            if (latestOtp.UsedAt != null)
            {
                return ServiceResult.Fail(ErrorCodes.Validation, "This verification code has already been used.");
            }

            if (latestOtp.ExpiresAt < DateTime.UtcNow)
            {
                return ServiceResult.Fail(ErrorCodes.Validation, "The verification code has expired. Please request a new one.");
            }

            if (latestOtp.Attempts >= MaxFailedAttempts)
            {
                return ServiceResult.Fail(ErrorCodes.Validation, "Too many incorrect attempts. Please generate a new code.");
            }

            string hashedInput = ComputeSha256Hash(submittedRawCode);
            if (hashedInput != latestOtp.OtpCodeHash)
            {
                latestOtp.Attempts++;
                await _otpRepository.UpdateAsync(latestOtp);
                await _otpRepository.SaveChangesAsync();

                int remainingAttempts = MaxFailedAttempts - latestOtp.Attempts;
                string hintMessage = remainingAttempts > 0
                    ? $"Invalid verification code. You have {remainingAttempts} attempts remaining."
                    : "Invalid verification code. This token is now locked.";

                return ServiceResult.Fail(ErrorCodes.Validation, hintMessage);
            }

            latestOtp.UsedAt = DateTime.UtcNow;
            await _otpRepository.UpdateAsync(latestOtp);
            await _otpRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        private static string ComputeSha256Hash(string rawData)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}