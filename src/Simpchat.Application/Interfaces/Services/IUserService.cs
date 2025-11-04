using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResult<List<SearchChatResponseDto>>> SearchAsync(string term, Guid userId);
        Task<ApiResult<GetByIdUserDto>> GetByIdAsync(Guid userId, Guid currentUserId);
        Task<ApiResult> UpdateAsync(Guid userId, UpdateUserDto updateUserInfoDto, UploadFileRequest avatar);
        Task<ApiResult> SetLastSeenAsync(Guid userId);
    }
}
