using MassTransit;
using MongoAuthApi.Contracts;
using System.Net;
using System.Net.Mail;

namespace MongoAuthApi.Consumers
{
    public class EmailConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailConsumer> _logger;

        // Inject Configuration to read appsettings.json
        public EmailConsumer(IConfiguration config, ILogger<EmailConsumer> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var user = context.Message;
            var settings = _config.GetSection("EmailSettings");

            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(settings["SenderEmail"]!),
                    Subject = "Welcome to Our Platform!",
                    Body = $"<h1>Hello {user.Username}!</h1><p>We are excited to have you on board.</p>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(user.Email);

                
                using var smtpClient = new SmtpClient(settings["SmtpHost"], int.Parse(settings["SmtpPort"]!))
                {
                    Credentials = new NetworkCredential(settings["SenderEmail"], settings["AppPassword"]),
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