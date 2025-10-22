using Simpchat.Application.Common.Pagination;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Search;
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
        Task<ApiResult> CreateAsync(Guid userId, PostChatDto chatPostDto, UploadFileRequest? avatar);
        Task<ApiResult> AddUserAsync(Guid chatId, Guid addingUserId, Guid currentUserId);
        Task<ApiResult> AddUserPermissionAsync(string permissionName, Guid chatId, Guid addingUserId, Guid currentUserId);
        Task<ApiResult> DeleteMemberAsync(Guid userId, Guid chatId);
        Task<ApiResult> DeleteAsync(Guid chatId);
        Task<ApiResult> UpdateAsync(Guid chatId, PostChatDto updateChatDto);
        Task<ApiResult<ICollection<SearchChatResponseDto>?>> SearchAsync(string searchTerm);
    }
}
