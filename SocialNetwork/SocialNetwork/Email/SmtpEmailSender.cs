using System.Net;
using System.Net.Mail;

namespace SocialNetwork.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration configuration, IHostEnvironment environment, ILogger<SmtpEmailSender> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var host = _configuration["Email:Host"];
            if (string.IsNullOrWhiteSpace(host))
            {
                if (_environment.IsDevelopment())
                {
                    _logger.LogInformation("Email to {Recipient}. Subject: {Subject}. Body: {Body}", to, subject, body);
                    return;
                }

                throw new InvalidOperationException("Email is not configured.");
            }

            var from = _configuration["Email:From"];
            if (string.IsNullOrWhiteSpace(from))
                from = "gather@localhost";

            var port = int.TryParse(_configuration["Email:Port"], out var parsedPort) ? parsedPort : 587;
            var enableSsl = !string.Equals(_configuration["Email:EnableSsl"], "false", StringComparison.OrdinalIgnoreCase);
            using var message = new MailMessage(from, to, subject, body);
            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl
            };

            var user = _configuration["Email:User"];
            var password = _configuration["Email:Password"];
            if (!string.IsNullOrWhiteSpace(user))
                client.Credentials = new NetworkCredential(user, password);

            await client.SendMailAsync(message);
        }
    }
}
