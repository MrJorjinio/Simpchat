using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IGroupService
    {
        Task<ApiResult> AddMemberAsync(Guid groupId, Guid userId);
        Task<ApiResult<Guid>> CreateAsync(PostChatDto chatPostDto, UploadFileRequest? avatar);
        Task<ApiResult> DeleteAsync(Guid groupId);
        Task<ApiResult> DeleteMemberAsync(Guid userId, Guid groupId);
        Task<ApiResult<List<SearchChatResponseDto>?>> SearchAsync(string searchTerm);
        Task<ApiResult> UpdateAsync(Guid groupId, PutChatDto updateChatDto, UploadFileRequest? avatar);
        Task<ApiResult<List<UserChatResponseDto>>> GetUserParticipatedAsync(Guid userId);
    }
}
