using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Application.Common.Models.Pagination;
using Simpchat.Application.Common.Models.Users;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResult<UserGetByIdDto>> GetByIdAsync(Guid id, Guid currentUserId);
        Task<ApiResult<ICollection<ChatSearchResponseDto>?>> SearchByUsernameAsync(string searchTerm, Guid userId);
        Task<ApiResult<UserResponseDto>> SetLastSeenAsync(Guid id);
        Task<ApiResult<UserResponseDto>> UpdateProfileAsync(Guid currentUserId, FileUploadRequest fileUploadRequest);
    }
}
