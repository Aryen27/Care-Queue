using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Repositories.Interfaces;
using carequeue.CQ.API.Services.Internal;

namespace carequeue.CQ.API.Services.Background
{
    public class NotificationBackgroundService: BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;
        private const int MaxRetryAttempts = 5;
        private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(30);
        public NotificationBackgroundService(
                    IServiceProvider serviceProvider,
                    ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        private static TimeSpan CalculateRetryDelay(
            int retryCount)
        {
            var minutes =
                Math.Min(
                    Math.Pow(2, retryCount - 1),
                    60);

            return TimeSpan.FromMinutes(minutes);
        }

        private async Task HandleRetryAsync(Notification notification, INotificationRepository repository)
        {
            notification.RetryCount++;

            if (notification.RetryCount >= MaxRetryAttempts)
            {
                notification.Status = NotificationStatus.Failed;
                notification.NextRetryAt = null;

                _logger.LogError(
                    "Notification {NotificationId} permanently failed after {RetryCount} attempts.",
                    notification.NotificationId,
                    notification.RetryCount);
            }
            else
            {
                notification.NextRetryAt = DateTime.UtcNow.Add(CalculateRetryDelay(notification.RetryCount));

                _logger.LogWarning(
                    "Notification {NotificationId} failed. Scheduled for retry at {NextRetryAt}. Attempt {RetryCount}/{MaxRetries}",
                    notification.NotificationId,
                    notification.NextRetryAt,
                    notification.RetryCount,
                    MaxRetryAttempts);
            }

            await repository.UpdateAsync(notification);
            await repository.SaveChangesAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a new scope for each batch run to safely resolve scoped services
                    using var scope = _serviceProvider.CreateScope();

                    var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    // Fetch all pending notifications
                    var pendingNotifications = await notificationRepository.GetPendingNotificationsAsync();

                    _logger.LogInformation("Processing {Count} pending notifications.", pendingNotifications.Count());

                    foreach (var notification in pendingNotifications)
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;

                        try
                        {
                            var result= await emailService.SendNotificationAsync(notification);

                            if (result.Success)
                            {
                                _logger.LogInformation("Notification {NotificationId} sent successfully.", notification.NotificationId);
                            }
                            else
                            {
                                await HandleRetryAsync(notification, notificationRepository);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Failed sending notification {NotificationId}",
                                notification.NotificationId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Catch exceptions so the worker doesn't crash entirely if a DB or network issue occurs
                    _logger.LogError(ex, "Error occurred in Notification Background Worker.");
                }

                try
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(30),
                        stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("Notification Background Worker stopping.");
        }
    }
}
