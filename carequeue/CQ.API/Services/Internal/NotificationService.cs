using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Repositories.Interfaces;

namespace carequeue.CQ.API.Services.Internal
{
    public class NotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationTemplateRepository _templateRepository;
        private readonly ICustomerRepository _customerRepository;

        public NotificationService(
            INotificationRepository notificationRepository,
            INotificationTemplateRepository templateRepository,
            ICustomerRepository customerRepository)
        {
            _notificationRepository = notificationRepository;
            _templateRepository = templateRepository;
            _customerRepository = customerRepository;
        }

        public async Task<ServiceResult<Notification>> CreateNotificationAsync(
            NotificationRequest request)
        {
            var template = await _templateRepository.GetByIdAsync(request.TemplateId);

            if (template == null)
            {
                return ServiceResult<Notification>.Fail(
                    ErrorCodes.NotFound,
                    "Notification template not found.");
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

            if (customer == null)
            {
                return ServiceResult<Notification>.Fail(
                    ErrorCodes.NotFound,
                    "Customer not found.");
            }

            var notification = new Notification
            {
                HospitalId = request.HospitalId,
                CustomerId = request.CustomerId,
                PatientId = request.PatientId,
                AppointmentId = request.AppointmentId,

                TemplateId = template.TemplateId,

                Channel = request.Channel,
                Status = NotificationStatus.Pending,

                Recipient = customer.Email,

                Subject = ReplaceTemplateValues(
                    template.Subject,
                    request.TemplateValues),

                Message = ReplaceTemplateValues(
                    template.Body,
                    request.TemplateValues),

                RetryCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            return ServiceResult<Notification>.Ok(notification);
        }

        public async Task<ServiceResult<IEnumerable<Notification>>> GetPendingNotificationsAsync()
        {
            var notifications =
                await _notificationRepository.GetPendingNotificationsAsync();

            return ServiceResult<IEnumerable<Notification>>.Ok(notifications);
        }

        public async Task<ServiceResult> MarkAsSentAsync(int notificationId)
        {
            var notification =
                await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Notification not found.");
            }

            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            notification.FailureReason = null;

            await _notificationRepository.UpdateAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> MarkAsFailedAsync(
            int notificationId,
            string failureReason)
        {
            var notification =
                await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Notification not found.");
            }

            notification.Status = NotificationStatus.Failed;
            notification.FailureReason = failureReason;
            notification.RetryCount++;
            notification.UpdatedAt = DateTime.UtcNow;

            await _notificationRepository.UpdateAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        private static string ReplaceTemplateValues(
            string template,
            Dictionary<string, string> values)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }

            foreach (var value in values)
            {
                template = template.Replace(
                    $"{{{{{value.Key}}}}}",
                    value.Value);
            }

            return template;
        }
    }
}