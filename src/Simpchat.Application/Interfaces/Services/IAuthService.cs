using Simpchat.Application.Models.ApiResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiResult<Guid>> RegisterAsync(string username, string password);
        Task<ApiResult<string>> LoginAsync(string username, string password);
        Task<ApiResult> UpdatePasswordAsync(Guid userId, string password);
    }
}
