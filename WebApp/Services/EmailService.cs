using Microsoft.Extensions.Options;
using System.Net.Mail;
using WebApp.Settings;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpSettings> smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            this.smtpSettings = smtpSettings;
        }
        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            using (var emailClient = new SmtpClient(smtpSettings.Value.Host, smtpSettings.Value.Port))
            {
                emailClient.Credentials =
                    new System.Net.NetworkCredential(smtpSettings.Value.User, smtpSettings.Value.Password);

                await emailClient.SendMailAsync(message);
            }
        }
    }
}
