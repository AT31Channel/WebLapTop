using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebLapTop.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public EmailSender(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            _smtpServer = configuration["EmailSettings:SmtpServer"] 
                ?? throw new InvalidOperationException("SMTP Server configuration is missing");
            
            var smtpPortStr = configuration["EmailSettings:SmtpPort"] 
                ?? throw new InvalidOperationException("SMTP Port configuration is missing");
            if (!int.TryParse(smtpPortStr, out _smtpPort))
            {
                throw new InvalidOperationException("Invalid SMTP Port configuration");
            }

            _smtpUsername = configuration["EmailSettings:SmtpUsername"] 
                ?? throw new InvalidOperationException("SMTP Username configuration is missing");
            
            _smtpPassword = configuration["EmailSettings:SmtpPassword"] 
                ?? throw new InvalidOperationException("SMTP Password configuration is missing");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            ArgumentException.ThrowIfNullOrEmpty(email);
            ArgumentException.ThrowIfNullOrEmpty(subject);
            ArgumentException.ThrowIfNullOrEmpty(htmlMessage);

            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUsername),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
} 