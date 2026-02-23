using Credio.Core.Application.Dtos.Email;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Credio.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
	{
		public MailSettings _mailSettings { get; }

		private readonly ILogger<EmailService> _logger;

		public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
		{
			_mailSettings = mailSettings.Value;
			_logger = logger;
		}

		public async Task SendAsync(EmailRequest request)
		{
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(request.From ?? _mailSettings.EmailFrom);
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                var builder = new BodyBuilder { HtmlBody = request.Body };

                // Ejemplo: añadir logo siempre que el template lo requiera
                var logoPath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates", "logo.jpeg");
                if (File.Exists(logoPath) && request.Body.Contains("cid:logoCid"))
                {
                    var image = builder.LinkedResources.Add(logoPath);
                    image.ContentId = "logoCid";
                    image.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
                }

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtp.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo electrónico");
            }
        }

	}
}
