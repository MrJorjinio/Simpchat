using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Simpchat.Application.Interfaces.Email
{
    public interface IEmailService
    {
        Task<bool> SendOtpAsync(string toEmail, string otp);
    }
}
