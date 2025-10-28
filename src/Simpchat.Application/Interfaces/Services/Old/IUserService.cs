using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Users.GetById;
using Simpchat.Application.Models.Users.Response;
using Simpchat.Application.Models.Users.Update;

namespace Simpchat.Application.Interfaces.Services.Old
{
    public interface IUserService
    {
        Task<ApiResult<GetByIdUserDto>> GetByIdAsync(Guid id, Guid currentUserId);
        Task<ApiResult> UpdateAvatarAsync(Guid userId, UploadFileRequest fileUploadRequest);
        Task<ApiResult> UpdateInfoAsync(Guid userId, UpdateUserInfoDto dto);
        Task<ApiResult<ICollection<SearchChatResponseDto>?>> SearchByUsernameAsync(string searchTerm, Guid userId);
        Task<ApiResult<UserResponseDto>> SetLastSeenAsync(Guid id);
    }
}
