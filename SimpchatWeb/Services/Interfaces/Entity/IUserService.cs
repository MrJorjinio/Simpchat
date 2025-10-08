using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Entity;

namespace SimpchatWeb.Services.Interfaces.Entity
{
    public interface IUserService
    {
        Task<ApiResult<UserGetByIdGetResponseDto>> GetUserByIdAsync(User user);
        Task<ApiResult<ICollection<UserSearchResponseDto>>> SearchByUsernameAsync(string username);
        Task<ApiResult<UserGetByIdGetResponseDto>> UpdateMyProfileAsync(User user, UserProfilePutDto model);
        Task<ApiResult<UserGetByIdGetResponseDto>> SetLastSeenAsync(User user);
        Task<ApiResult> UpdateMyPasswordAsync(User user, UserPutPasswordDto model);
        Task<ApiResult> DeleteMeAsync(User user);
    }
}
