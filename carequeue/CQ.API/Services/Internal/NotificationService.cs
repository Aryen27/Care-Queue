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
            var template = await _templateRepository.GetByTypeAsync(request.TemplateType.ToString());

            if (template == null)
            {
                return ServiceResult<Notification>.Fail(
                    ErrorCodes.NotFound,
                    $"Notification template for type '{request.TemplateType}' not found.");
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

            // HTTP Request ends here! 
            // The NotificationBackgroundService will pick this up and handle the SMTP transfer.
            return ServiceResult<Notification>.Ok(notification);
        }

        public async Task<ServiceResult<IEnumerable<Notification>>> GetPendingNotificationsAsync()
        {
            var notifications =
                await _notificationRepository.GetPendingNotificationsAsync();

            return ServiceResult<IEnumerable<Notification>>.Ok(notifications);
        }

        private static string ReplaceTemplateValues(
            string template,
            Dictionary<string, string> values)
        {
            if (string.IsNullOrWhiteSpace(template) || values == null || values.Count == 0)
            {
                return template ?? string.Empty;
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