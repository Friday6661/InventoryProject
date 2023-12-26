using MailKit.Net.Smtp;
using InventoryProject.Contracts;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using MailKit.Security;
using MailKit;
using MimeKit;
using Microsoft.Extensions.Options;

namespace InventoryProject.Repositories
{
    public class EmailServiceRepository : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailServiceRepository(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }
        public async Task SendEmailAsync(SendEmailDTO sendEmailDTO)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(sendEmailDTO.ToEmail));
            email.Subject = sendEmailDTO.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = sendEmailDTO.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public string LoadEmailTemplate(string templateFilePath)
        {
            return File.ReadAllText(templateFilePath);
        }

        public string ReplacePlaceholders(string template, Dictionary<string, string> replacements)
        {
            foreach (var replacement in replacements)
            {
                template = template.Replace($"{{{replacement.Key}}}", replacement.Value);
            }
            return template;
        }
    }
}