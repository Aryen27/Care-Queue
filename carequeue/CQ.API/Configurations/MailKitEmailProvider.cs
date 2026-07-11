using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace carequeue.CQ.API.Configurations
{
    public class MailKitEmailProvider
    {
        private readonly SmtpSettings _smtpSettings;

        public MailKitEmailProvider(
            IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendAsync(
            EmailMessage message)
        {
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _smtpSettings.SenderName,
                    _smtpSettings.SenderEmail));

            email.To.Add(
                MailboxAddress.Parse(
                    message.Recipient));

            email.Subject = message.Subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = message.IsHtml
                    ? message.Body
                    : null,

                TextBody = message.IsHtml
                    ? null
                    : message.Body
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            var socketOption =
                _smtpSettings.UseSSL
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

            await smtp.ConnectAsync(
                _smtpSettings.Host,
                _smtpSettings.Port,
                socketOption);

            if (!string.IsNullOrWhiteSpace(_smtpSettings.Username))
            {
                await smtp.AuthenticateAsync(
                    _smtpSettings.Username,
                    _smtpSettings.Password);
            }

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}