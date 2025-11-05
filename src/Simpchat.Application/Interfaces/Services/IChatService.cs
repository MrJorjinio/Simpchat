using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats;
using Simpchat.Domain.Enums;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<ApiResult> UpdatePrivacyTypeAsync(Guid chatId, ChatPrivacyType chatPrivacyType);
        Task<ApiResult<GetByIdChatProfile>> GetProfileAsync(Guid chatId, Guid userId);
        Task<ApiResult<GetByIdChatDto>> GetByIdAsync(Guid chatId, Guid userId);
        Task<ApiResult<List<UserChatResponseDto>>> GetUserChatsAsync(Guid userId);
        Task<ApiResult<List<SearchChatResponseDto>>> SearchAsync(string term, Guid userId);
        Task<ApiResult<Guid>> AddUserPermissionAsync(Guid userId, Guid chatId, string permissionName);
        
    }
}
