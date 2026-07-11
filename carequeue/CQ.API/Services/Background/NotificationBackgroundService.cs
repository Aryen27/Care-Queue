using carequeue.CQ.API.Repositories.Interfaces;
using carequeue.CQ.API.Services.Internal;

namespace carequeue.CQ.API.Services.Background
{
    public class NotificationBackgroundService: BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;
        public NotificationBackgroundService(
                    IServiceProvider serviceProvider,
                    ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
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
                                _logger.LogWarning("Notification {NotificationId} failed to send. Reason: {Error}",
                                    notification.NotificationId, result.Error?.Message);
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
