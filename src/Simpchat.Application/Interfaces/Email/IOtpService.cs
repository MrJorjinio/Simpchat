using Simpchat.Application.Models.ApiResult;
using Simpchat.Domain.Entities;
using Simpchat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Email
{
    public interface IOtpService
    {
        Task<Result> SendAndSaveUserOtpAsync(Guid userId);
        Task<Result> SendAndSaveEmailOtpAsync(string email);
        Task<Result<bool>> ValidateUserOtpAsync(Guid userId, string otpCode);
        Task<Result<bool>> ValidateEmailOtpAsync(string email, string otpCode);
    }
}
