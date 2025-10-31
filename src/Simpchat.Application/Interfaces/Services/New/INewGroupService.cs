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
    internal interface INewGroupService
    {
        Task<ApiResult> AddMemberAsync(Guid groupId, Guid userId);
        Task<ApiResult> CreateAsync(PostChatDto chatPostDto);
        Task<ApiResult> DeleteAsync(Guid groupId);
        Task<ApiResult> DeleteMemberAsync(Guid userId, Guid groupId);
        Task<ApiResult<List<SearchChatResponseDto>?>> SearchAsync(string searchTerm);
        Task<ApiResult> UpdateAsync(Guid groupId, PostChatDto updateChatDto);
        Task<ApiResult<List<UserChatResponseDto>>> GetUserSubscribedAsync(Guid userId);
    }
}
