using Simpchat.Application.Models.ApiResults;

namespace Simpchat.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ApiResult> RegisterAsync(string username, string password);
        Task<ApiResult<string>> LoginAsync(string username, string password);
        Task<ApiResult> UpdatePasswordAsync(Guid userId, string password);
    }
}
