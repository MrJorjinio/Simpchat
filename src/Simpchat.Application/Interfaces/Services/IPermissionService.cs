using Simpchat.Application.Models.Permissions;
using Simpchat.Shared.Models;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<Result<Guid>> GrantPermissionAsync(GrantPermissionDto dto, Guid requesterId);
        Task<Result> RevokePermissionAsync(RevokePermissionDto dto, Guid requesterId);
        Task<Result<UserChatPermissionsResponseDto>> GetUserPermissionsAsync(Guid chatId, Guid userId, Guid requesterId);
        Task<Result<List<UserChatPermissionsResponseDto>>> GetAllChatPermissionsAsync(Guid chatId, Guid requesterId);
        Task<Result> RevokeAllPermissionsAsync(Guid chatId, Guid userId, Guid requesterId);
    }
}
