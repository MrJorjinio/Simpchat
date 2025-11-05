using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IChannelService
    {
        Task<ApiResult> AddSubscriberAsync(Guid channelId, Guid userId);
        Task<ApiResult<Guid>> CreateAsync(PostChatDto chatPostDto, UploadFileRequest? avatar);
        Task<ApiResult> DeleteAsync(Guid channelId);
        Task<ApiResult> DeleteSubscriberAsync(Guid userId, Guid channelId);
        Task<ApiResult<List<SearchChatResponseDto>?>> SearchAsync(string searchTerm);
        Task<ApiResult> UpdateAsync(Guid channelId, UpdateChatDto updateChatDto, UploadFileRequest? avatar);
        Task<ApiResult<List<UserChatResponseDto>>> GetUserSubscribedAsync(Guid userId);
    }
}
