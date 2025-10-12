using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.Files;
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
        Task<ApiResult<UserResponseDto>> GetByIdAsync(Guid id);
        Task<ApiResult<ICollection<UserResponseDto>?>> SearchByUsernameAsync(string searchTerm);
        Task<ApiResult<UserResponseDto>> SetLastSeenAsync(Guid id);
        Task<ApiResult<UserResponseDto>> UpdateProfileAsync(Guid id, FileUploadRequest fileUploadRequest);
    }
}
