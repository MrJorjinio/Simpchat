using Simpchat.Application.Common.Pagination;
using Simpchat.Application.Common.Pagination.Chat;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats.Get.ById;
using Simpchat.Application.Models.Chats.Get.Profile;
using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
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
        Task<ApiResult<PaginationResult<SearchChatResponseDto>>> SearchByNameAsync(ChatSearchPageModel chatSearchDto, Guid currentUserId);
        Task<ApiResult<ICollection<UserChatResponseDto>?>> GetUserChatsAsync(Guid userId);
        Task<ApiResult<GetByIdChatDto>> GetByIdAsync(Guid chatId, Guid userId);
        Task<ApiResult<GetByIdChatProfile>> GetProfileByIdAsync(Guid chatId, Guid userId);
        Task<ApiResult> UpdateAvatarAsync(Guid chatId, Guid userId, UploadFileRequest file);
        Task<ApiResult> UpdatePrivacyTypeAsync(Guid chatId, Guid userId, ChatPrivacyType privacyType);
    }
}
