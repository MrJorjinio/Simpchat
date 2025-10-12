using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Entity;
using SimpchatWeb.Services.Interfaces.Minio;
using System.Net.Mime;
using System.Security.AccessControl;

namespace SimpchatWeb.Services.Entity
{
    public class UserService : IUserService
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "profile-pics";

        public UserService(
            SimpchatDbContext dbContext,
            IMapper mapper,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IFileStorageService fileStorageService
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _fileStorageService = fileStorageService;
        }
        public async Task<ApiResult> DeleteMeAsync(User user)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            var response = new ApiResult(true, 200, "Success");
            return response;
        }

        public async Task<ApiResult<UserGetByIdGetResponseDto>> GetUserByIdAsync(User user)
        {
            var response = _mapper.Map<UserGetByIdGetResponseDto>(user);
            return new ApiResult<UserGetByIdGetResponseDto>(true, 200, "Success", response);
        }

        public async Task<ApiResult<ICollection<UserSearchResponseDto>>> SearchByUsernameAsync(string username)
        {
            var similarUsers = await _dbContext.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{username}%"))
                .ToListAsync();

            var response = _mapper.Map<ICollection<UserSearchResponseDto>>(similarUsers);
            return new ApiResult<ICollection<UserSearchResponseDto>>(true, 200, "Success", response);
        }

        public async Task<ApiResult<UserGetByIdGetResponseDto>> SetLastSeenAsync(User user)
        {
            user.LastSeen = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<UserGetByIdGetResponseDto>(user);
            response.IsOnline = user.LastSeen.AddSeconds(10) > DateTimeOffset.UtcNow;

            return new ApiResult<UserGetByIdGetResponseDto>(true, 200, "Success", response);
        }

        public async Task<ApiResult> UpdateMyPasswordAsync(User user, UserPutPasswordDto model)
        {
            if (_passwordHasher.Verify(model.CurrentPassword, user.Salt, user.PasswordHash) is false)
            {
                return new ApiResult(false, 400, "Invalid password");
            }

            var newPasswordHash = _passwordHasher.Encrypt(model.NewPassword, user.Salt);
            user.PasswordHash = newPasswordHash;
            await _dbContext.SaveChangesAsync();

            return new ApiResult(true, 200, "Success");
        }

        public async Task<ApiResult<UserGetByIdGetResponseDto>> UpdateMyProfileAsync(User user, UserProfilePutDto model)
        {
            user = _mapper.Map(model, user);
            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<UserGetByIdGetResponseDto>(user);
            return new ApiResult<UserGetByIdGetResponseDto>(true, 200, "Success", response);
        }
    }
}
