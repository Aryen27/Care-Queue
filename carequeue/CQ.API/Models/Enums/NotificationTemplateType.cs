namespace carequeue.CQ.API.Models.Enums
{
    public enum NotificationTemplateType
    {
        Scheduled,
        Completed,
        Cancelled,
        Rescheduled,
        NoShow,

        EmailVerification,
        PasswordReset,
        Payment,
        Login,

        Created,
        Authorized,
        Captured,
        Refunded,
        Failed
    }
}
