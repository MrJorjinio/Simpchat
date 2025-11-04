using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiResult<Guid>> RegisterAsync(RegisterUserDto registerUserDto);
        Task<ApiResult<string>> LoginAsync(LoginUserDto loginUserDto);
        Task<ApiResult> UpdatePasswordAsync(Guid userId, UpdatePasswordDto updatePasswordDto);
        Task<ApiResult> ResetPasswordAsync(Guid userId, ResetPasswordDto resetPasswordDto);
    }
}
