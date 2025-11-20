using Microsoft.Extensions.Options;
using Simpchat.Application.Interfaces.Email;
using Simpchat.Shared.Config;
using Simpchat.Shared.Models;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Simpchat.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _config;

        public EmailService(EmailSettings emailSettings)
        {
            _config = emailSettings;
        }
        public async Task<Result> SendOtpAsync(string toEmail, string otpCode)
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
                Body = GenerateBody(otpCode),
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
            return Result.Success();
        }

        private string GenerateBody(string otp)
        {
            var safeOtp = WebUtility.HtmlEncode(otp);

            var sb = new StringBuilder();

            sb.AppendLine($@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
              <meta charset=""utf-8"">
              <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
              <meta name=""color-scheme"" content=""light dark"">
              <meta name=""supported-color-schemes"" content=""light dark"">
              <title>Verify Your Simpchat Account</title>
            </head>
            <body style=""margin:0;padding:0;background:#0a0a0a;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;"">
  
              <!-- Preheader -->
              <div style=""display:none;font-size:1px;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;"">
                Your Simpchat verification code. Chat freely with secure verification.
              </div>

              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"" style=""background:#0a0a0a;padding:50px 20px;"">
                <tr>
                  <td align=""center"">
        
                    <!-- Main Container -->
                    <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"" style=""max-width:600px;width:100%;background:#141414;border:1px solid #1f1f1f;overflow:hidden;"">
  
                      <!-- Header -->
                      <tr>
                        <td style=""background:#0a0a0a;padding:60px 50px;text-align:center;border-bottom:1px solid #1f1f1f;"">
              
                          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"">
                            <tr>
                              <td align=""center"">
                                <!-- Brand Name -->
                                <h1 style=""margin:0 0 12px 0;color:#ffffff;font-size:48px;font-weight:300;letter-spacing:14px;text-transform:uppercase;"">SIMPCHAT</h1>
                                <p style=""margin:0;color:#666666;font-size:13px;letter-spacing:6px;text-transform:uppercase;font-weight:300;"">CHAT FREELY</p>
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>

                      <!-- Content -->
                      <tr>
                        <td style=""padding:60px 50px;"">
              
                          <!-- Title -->
                          <h2 style=""margin:0 0 24px 0;font-size:22px;font-weight:400;color:#ffffff;line-height:1.4;letter-spacing:2px;text-transform:uppercase;"">Email Verification</h2>
              
                          <p style=""margin:0 0 40px 0;font-size:15px;color:#8a8a8a;line-height:1.8;font-weight:300;"">To complete your registration and start connecting with others, please verify your email address using the code below.</p>

                          <!-- OTP Code Box -->
                          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"" style=""margin-bottom:45px;"">
                            <tr>
                              <td align=""center"">
                                <table cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"" style=""background:#0a0a0a;border:1px solid #2a2a2a;padding:50px 55px;"">
                                  <tr>
                                    <td style=""text-align:center;"">
                                      <p style=""margin:0 0 22px 0;color:#4a4a4a;font-size:10px;text-transform:uppercase;letter-spacing:4px;font-weight:600;"">VERIFICATION CODE</p>
                                      <p style=""margin:0 0 18px 0;color:#ffffff;font-size:56px;font-weight:700;letter-spacing:18px;font-family:'Courier New',Courier,monospace;"">{otp}</p>
                                      <p style=""margin:0;color:#3a3a3a;font-size:11px;font-weight:400;letter-spacing:2px;text-transform:uppercase;"">Valid for 5 minutes</p>
                                    </td>
                                  </tr>
                                </table>
                              </td>
                            </tr>
                          </table>

                          <!-- Verify Button -->
                          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"" style=""margin-bottom:50px;"">
                            <tr>
                              <td align=""center"">
                                <a href=""#"" style=""display:inline-block;background:#ffffff;color:#0a0a0a;text-decoration:none;padding:20px 60px;font-size:12px;font-weight:600;letter-spacing:3px;text-transform:uppercase;border:none;transition:all 0.3s;"">
                                  VERIFY ACCOUNT
                                </a>
                              </td>
                            </tr>
                          </table>

                          <!-- Divider -->
                          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"" style=""margin:45px 0;"">
                            <tr>
                              <td style=""border-top:1px solid #1f1f1f;""></td>
                            </tr>
                          </table>

                          <!-- Security Info -->
                          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"">
                            <tr>
                              <td style=""background:#0a0a0a;border-left:1px solid #3a3a3a;padding:32px 36px;border:1px solid #1a1a1a;"">
                                <p style=""margin:0 0 24px 0;font-weight:500;color:#b0b0b0;font-size:12px;letter-spacing:3px;text-transform:uppercase;"">Security Guidelines</p>
                    
                                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"">
                                  <tr>
                                    <td style=""padding-bottom:14px;"">
                                      <p style=""margin:0;color:#6a6a6a;font-size:14px;line-height:1.8;font-weight:300;"">
                                        This verification code expires in 5 minutes
                                      </p>
                                    </td>
                                  </tr>
                                  <tr>
                                    <td style=""padding-bottom:14px;"">
                                      <p style=""margin:0;color:#6a6a6a;font-size:14px;line-height:1.8;font-weight:300;"">
                                        Never share this code with anyone, including Simpchat staff
                                      </p>
                                    </td>
                                  </tr>
                                  <tr>
                                    <td style=""padding-bottom:14px;"">
                                      <p style=""margin:0;color:#6a6a6a;font-size:14px;line-height:1.8;font-weight:300;"">
                                        We will never ask you for this verification code
                                      </p>
                                    </td>
                                  </tr>
                                  <tr>
                                    <td>
                                      <p style=""margin:0;color:#6a6a6a;font-size:14px;line-height:1.8;font-weight:300;"">
                                        If you didn't request this code, please ignore this email
                                      </p>
                                    </td>
                                  </tr>
                                </table>
                              </td>
                            </tr>
                          </table>

                        </td>
                      </tr>

                      <!-- Footer -->
                      <tr>
                        <td style=""background:#0a0a0a;padding:45px 50px;text-align:center;border-top:1px solid #1f1f1f;"">
              
                          <p style=""margin:0 0 28px 0;color:#3a3a3a;font-size:12px;line-height:1.8;font-weight:300;"">
                            Questions? Contact us at <a href=""mailto:support@simpchat.com"" style=""color:#6a6a6a;text-decoration:none;font-weight:400;"">support@simpchat.com</a>
                          </p>

                          <!-- Links -->
                          <table cellpadding=""0"" cellspacing=""0"" border=""0"" role=""presentation"" style=""margin:0 auto 28px auto;"">
                            <tr>
                              <td style=""padding:0 16px;"">
                                <a href=""#"" style=""color:#4a4a4a;text-decoration:none;font-size:11px;font-weight:400;letter-spacing:2px;text-transform:uppercase;"">Privacy</a>
                              </td>
                              <td style=""color:#2a2a2a;padding:0 6px;"">•</td>
                              <td style=""padding:0 16px;"">
                                <a href=""#"" style=""color:#4a4a4a;text-decoration:none;font-size:11px;font-weight:400;letter-spacing:2px;text-transform:uppercase;"">Terms</a>
                              </td>
                              <td style=""color:#2a2a2a;padding:0 6px;"">•</td>
                              <td style=""padding:0 16px;"">
                                <a href=""#"" style=""color:#4a4a4a;text-decoration:none;font-size:11px;font-weight:400;letter-spacing:2px;text-transform:uppercase;"">Support</a>
                              </td>
                            </tr>
                          </table>

                          <p style=""margin:0 0 8px 0;color:#2a2a2a;font-size:11px;line-height:1.7;font-weight:300;"">
                            This is an automated message from Simpchat
                          </p>
              
                          <p style=""margin:0;color:#1a1a1a;font-size:10px;font-weight:300;letter-spacing:2px;text-transform:uppercase;"">&copy; 2025 Simpchat Inc.</p>
                        </td>
                      </tr>

                    </table>
        
                  </td>
                </tr>
              </table>
            </body>
            </html>");

            return sb.ToString();
        }
    }
}
