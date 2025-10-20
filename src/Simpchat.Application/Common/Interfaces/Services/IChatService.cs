using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.Chats.Get.ById;
using Simpchat.Application.Common.Models.Chats.Get.Profile;
using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Application.Common.Models.Chats.Post.Message;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Application.Common.Models.Pagination;
using Simpchat.Application.Common.Models.Pagination.Chat;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Services
{
    public interface IChatService
    {
        Task<ApiResult<PaginationResult<ChatSearchResponseDto>>> SearchByNameAsync(ChatSearchPageModel chatSearchDto, Guid currentUserId);
        Task<ApiResult<ICollection<UserChatResponseDto>?>> GetUserChatsAsync(Guid userId);
        Task<ApiResult<ChatGetByIdDto>> GetByIdAsync(Guid chatId, Guid userId);
        Task<ApiResult<ChatGetByIdProfile>> GetProfileByIdAsync(Guid chatId, Guid userId);
        Task<ApiResult> SendMessageAsync(MessagePostDto message, Guid currentUserId);
        Task<ApiResult> UpdateAvatarAsync(Guid chatId, Guid userId, FileUploadRequest file);
        Task<ApiResult> UpdatePrivacyTypeAsync(Guid chatId, Guid userId, ChatPrivacyType privacyType);
    }
}
