using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Chats.Get.ById;
using Simpchat.Application.Models.Chats.Get.Profile;
using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
