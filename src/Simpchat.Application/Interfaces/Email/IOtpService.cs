using Simpchat.Application.Models.ApiResult;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Email
{
    public interface IOtpService
    {
        Task<string> GenerateAndSaveUserOtpAsync(Guid userId);
        Task<string> ValidateOtpCodeAsync(Guid userId, string code);
        Task<string> GenerateAndSaveEmailOtpAsync(string email);
        Task<string> GetEmailOtpAsync(string email);
    }
}
