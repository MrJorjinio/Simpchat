using Microsoft.Extensions.Options;
using Simpchat.Application.Interfaces.Email;
using Simpchat.Shared.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Simpchat.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _config;

        public EmailService(IOptions<AppSettings> config)
        {
            _config = config.Value.EmailSettings;
        }
        public async Task<bool> SendOtpAsync(string toEmail, string otp)
        {
            using var client = new SmtpClient(_config.SmtpServer, _config.Port)
            {
                EnableSsl = _config.EnableSsl,
                Credentials = new NetworkCredential(_config.Username, _config.Password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(_config.DefaultFromEmail, _config.DefaultFromName),
                Subject = "Simpchat: OTP Verification Code",
                Body = GenerateBody(otp),
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
            return true;
        }

        private string GenerateBody(string otp)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body style='font-family:sans-serif;'>");
            sb.AppendLine("<h3>Welcome to Simpchat!</h3>");
            sb.AppendLine("<p>Your one-time verification code is:</p>");
            sb.AppendLine($"<div style='font-size: 24px; font-weight: bold; margin: 20px 0;'>{otp}</div>");
            sb.AppendLine("<p>Please do not share this code with anyone. It will expire in 5 minutes.</p>");
            sb.AppendLine("<p>If you did not request this, please ignore.</p>");
            sb.AppendLine("<br/><small>&copy; 2025 Simpchat</small>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }
    }
}
