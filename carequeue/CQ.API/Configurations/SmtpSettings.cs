namespace carequeue.CQ.API.Configurations
{
    public class SmtpSettings
    {
        public const string SectionName = "SmtpSettings";

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; }

        public bool UseSSL { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string SenderName { get; set; } = string.Empty;

        public string SenderEmail { get; set; } = string.Empty;
    }
}