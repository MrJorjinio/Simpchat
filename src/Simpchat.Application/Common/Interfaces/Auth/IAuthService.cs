using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.Users;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ApiResult> RegisterAsync(string username, string password);
        Task<ApiResult<string>> LoginAsync(string username, string password);
        Task<ApiResult> UpdatePasswordAsync(Guid userId, string password);
    }
}
