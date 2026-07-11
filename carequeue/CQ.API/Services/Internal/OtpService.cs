using System.Security.Cryptography;
using System.Text;
using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Services.Internal
{
    public class OtpService
    {
        private readonly AppDbContext _context;
        private readonly IOtpVerificationRepository _otpRepository;
        private readonly NotificationService _notificationService;

        private const int MaxFailedAttempts = 3;
        private const int ExpirationMinutes = 10;
        private const int CooldownSeconds = 60;

        public OtpService(
            AppDbContext context,
            IOtpVerificationRepository otpRepository,
            NotificationService notificationService)
        {
            _context = context;
            _otpRepository = otpRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<string>> GenerateAndSendOtpAsync(
            int customerId,
            OtpPurpose purpose)
        {
            // Cooldown validation
            var latestOtp =
                await _otpRepository.GetLatestActiveOtpAsync(
                    customerId,
                    purpose);

            if (latestOtp != null)
            {
                var elapsed =
                    (DateTime.UtcNow - latestOtp.CreatedAt).TotalSeconds;

                if (elapsed < CooldownSeconds)
                {
                    var remaining =
                        CooldownSeconds - (int)elapsed;

                    return ServiceResult<string>.Fail(
                        ErrorCodes.Validation,
                        $"Please wait {remaining} seconds before requesting another verification code.");
                }
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Generate secure OTP
                string rawCode =
                    RandomNumberGenerator
                        .GetInt32(100000, 1000000)
                        .ToString();

                string hashedCode =
                    ComputeSha256Hash(rawCode);

                var otpVerification =
                    new OtpVerification
                    {
                        CustomerId = customerId,
                        OtpCodeHash = hashedCode,
                        Purpose = purpose,

                        ExpiresAt =
                            DateTime.UtcNow.AddMinutes(
                                ExpirationMinutes),

                        CreatedAt = DateTime.UtcNow,

                        Attempts = 0,

                        UsedAt = null,
                        RevokedAt = null
                    };

                await _otpRepository.AddAsync(
                    otpVerification);

                await _otpRepository.SaveChangesAsync();

                var notification =
                    await _notificationService
                        .CreateNotificationAsync(
                            new NotificationRequest
                            {
                                HospitalId = 0,

                                CustomerId = customerId,

                                PatientId = Guid.Empty,

                                TemplateType =
                                    purpose switch
                                    {
                                        OtpPurpose.EmailVerification =>
                                            NotificationTemplateType.EmailVerification,

                                        OtpPurpose.PasswordReset =>
                                            NotificationTemplateType.PasswordReset,

                                        OtpPurpose.Login =>
                                            NotificationTemplateType.Login,

                                        OtpPurpose.Payment =>
                                            NotificationTemplateType.Payment,

                                        _ =>
                                            throw new InvalidOperationException()
                                    },

                                TemplateValues =
                                    new()
                                    {
                                        ["OTP"] = rawCode,
                                        ["ExpiryMinutes"] =
                                            ExpirationMinutes.ToString()
                                    }
                            });

                if (!notification.Success)
                {
                    await transaction.RollbackAsync();

                    return ServiceResult<string>.Fail(
                        notification.Error.Code,
                        notification.Error.Message!);
                }

                // Revoke previous active OTPs only after
                // the new OTP has been successfully emailed.
                var activeOtps =
                    await _otpRepository
                        .GetActiveOtpsAsync(
                            customerId,
                            purpose);

                foreach (var otp in activeOtps)
                {
                    if (otp.OtpId == otpVerification.OtpId)
                    {
                        continue;
                    }

                    otp.RevokedAt = DateTime.UtcNow;

                    await _otpRepository.UpdateAsync(otp);
                }

                await _otpRepository.SaveChangesAsync();

                await transaction.CommitAsync();

                return ServiceResult<string>.Ok(rawCode);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ServiceResult> VerifyOtpAsync(
            int customerId,
            OtpPurpose purpose,
            string submittedRawCode)
        {
            if (string.IsNullOrWhiteSpace(submittedRawCode))
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Verification code cannot be empty.");
            }

            var latestOtp =
                await _otpRepository
                    .GetLatestOtpAsync(
                        customerId,
                        purpose);

            if (latestOtp == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "No verification token found for this account.");
            }

            if (latestOtp.UsedAt != null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "This verification code has already been used.");
            }

            if (latestOtp.RevokedAt != null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "This verification code has been superseded by a newer code.");
            }

            if (latestOtp.ExpiresAt < DateTime.UtcNow)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "The verification code has expired. Please request a new one.");
            }

            if (latestOtp.Attempts >= MaxFailedAttempts)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Too many incorrect attempts. Please generate a new verification code.");
            }

            string hashedInput =
                ComputeSha256Hash(submittedRawCode);

            if (hashedInput != latestOtp.OtpCodeHash)
            {
                latestOtp.Attempts++;

                await _otpRepository.UpdateAsync(latestOtp);
                await _otpRepository.SaveChangesAsync();

                int remainingAttempts =
                    MaxFailedAttempts - latestOtp.Attempts;

                var message =
                    remainingAttempts > 0
                        ? $"Invalid verification code. You have {remainingAttempts} attempts remaining."
                        : "Invalid verification code. This token is now locked.";

                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    message);
            }

            latestOtp.UsedAt = DateTime.UtcNow;

            await _otpRepository.UpdateAsync(latestOtp);
            await _otpRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        private static string ComputeSha256Hash(
            string rawData)
        {
            byte[] bytes =
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new();

            foreach (var b in bytes)
            {
                builder.Append(
                    b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}