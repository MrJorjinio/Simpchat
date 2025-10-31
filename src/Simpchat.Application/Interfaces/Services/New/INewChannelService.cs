using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services.New
{
    public interface INewChannelService
    {
        Task<ApiResult> AddSubscriberAsync(Guid channelId, Guid userId);
        Task<ApiResult> CreateAsync(PostChatDto chatPostDto);
        Task<ApiResult> DeleteAsync(Guid channelId);
        Task<ApiResult> DeleteSubscriberAsync(Guid userId, Guid channelId);
        Task<ApiResult<List<SearchChatResponseDto>?>> SearchAsync(string searchTerm);
        Task<ApiResult> UpdateAsync(Guid channelId, PostChatDto updateChatDto);
        Task<ApiResult<List<UserChatResponseDto>>> GetUserSubscribedAsync(Guid userId);
    }
}
