using MassTransit;
using MongoAuthApi.Contracts;
using MongoAuthApi.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MongoAuthApi.Consumers
{
    public class EmailConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailConsumer> _logger;

        // Inject Configuration to read appsettings.json
        public EmailConsumer(IOptions<EmailSettings> emailSettings, ILogger<EmailConsumer> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var user = context.Message;

            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = "Welcome to Our Platform!",
                    Body = $"<h1>Hello {user.Username}!</h1><p>We are excited to have you on board.</p>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(user.Email);

                
                using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.AppPassword),
                    EnableSsl = true,
                };

                // Send the Email
                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending email: {Message}", ex.Message);
            }
        }
    }
}