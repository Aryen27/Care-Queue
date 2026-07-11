namespace carequeue.CQ.API.Configurations
{
    public class EmailMessage
    {
        public string Recipient { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public bool IsHtml { get; set; } = true;
    }
}