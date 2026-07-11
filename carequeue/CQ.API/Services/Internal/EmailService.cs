using carequeue.CQ.API.Configurations;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Repositories;
using carequeue.CQ.API.Repositories.Interfaces;

namespace carequeue.CQ.API.Services.Internal
{
    public class EmailService
    {
        private readonly MailKitEmailProvider _provider;
        private readonly EmailLogRepository _emailLogRepository;
        private readonly INotificationRepository _notificationRepository;

        public EmailService(
            MailKitEmailProvider provider,
            EmailLogRepository emailLogRepository,
            INotificationRepository notificationRepository)
        {
            _provider = provider;
            _emailLogRepository = emailLogRepository;
            _notificationRepository = notificationRepository;
        }

        private async Task MarkNotificationSentAsync(
    Notification notification)
        {
            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            notification.FailureReason = null;

            await _notificationRepository.UpdateAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }

        private async Task MarkNotificationFailedAsync(
            Notification notification,
            string reason)
        {
            notification.Status = NotificationStatus.Failed;
            notification.FailureReason = reason;
            notification.RetryCount++;
            notification.UpdatedAt = DateTime.UtcNow;

            await _notificationRepository.UpdateAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }

        public async Task<ServiceResult> SendNotificationAsync(
            Notification notification)
        {
            var emailLog = new EmailLog
            {
                NotificationId = notification.NotificationId,
                Recipient = notification.Recipient,
                Subject = notification.Subject ?? string.Empty,
                Provider = "MailKit SMTP",
                Status = EmailStatus.Pending
            };

            await _emailLogRepository.AddAsync(emailLog);
            await _emailLogRepository.SaveChangesAsync();

            try
            {
                await _provider.SendAsync(
                    new EmailMessage
                    {
                        Recipient = notification.Recipient,
                        Subject = notification.Subject ?? string.Empty,
                        Body = notification.Message,
                        IsHtml = true
                    });

                emailLog.Status = EmailStatus.Sent;
                emailLog.SentAt = DateTime.UtcNow;

                await _emailLogRepository.UpdateAsync(emailLog);
                await _emailLogRepository.SaveChangesAsync();

                await MarkNotificationSentAsync(notification);

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                emailLog.Status = EmailStatus.Failed;
                emailLog.FailureReason = ex.Message;

                await _emailLogRepository.UpdateAsync(emailLog);
                await _emailLogRepository.SaveChangesAsync();

                await MarkNotificationFailedAsync(
                    notification,
                    ex.Message);

                return ServiceResult.Fail(
                    ErrorCodes.ServerError,
                    $"Email sending failed: {ex.Message}");
            }
        }
    }
}