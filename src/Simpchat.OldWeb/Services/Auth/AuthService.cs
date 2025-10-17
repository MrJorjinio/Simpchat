using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Minio;
using SimpchatWeb.Services.Settings;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SimpchatWeb.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AppSettings _appSettings;
        private readonly IFileStorageService _fileStorageSystem;
        private const string BucketName = "profile-pics";
        public AuthService(
            SimpchatDbContext dbContext,
            IPasswordHasher passwordHasher,
            IOptions<AppSettings> options,
            IFileStorageService fileStorageService
            )
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _appSettings = options.Value;
            _fileStorageSystem = fileStorageService;
        }

        public async Task<bool> RegisterAsync(UserRegisterPostDto model)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == model.Username))
            {
                return false;
            }

            string salt = Guid.NewGuid().ToString();
            string passwordHash = _passwordHasher.Encrypt(model.Password, salt);

            var user = new User()
            {
                Username = model.Username,
                PasswordHash = passwordHash,
                Salt = salt,
                Description = string.Empty,
                ChatMemberAddPermissionType = ChatMemberAddPermissionType.WithConversations
            };

            if (model.ProfilePicture != null && model.ProfilePicture.Length != 0)
            {
                var fileExtention = Path.GetExtension(model.ProfilePicture.FileName);
                var objectName = $"{Guid.NewGuid()}{fileExtention}";

                using (var stream = model.ProfilePicture.OpenReadStream())
                {
                    var fileUrl = await _fileStorageSystem.UploadFileAsync(BucketName, objectName, stream, fileExtention);
                    user.ProfilePictureUrl = fileUrl;
                }
            }

            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var defaultRole = await _dbContext.GlobalRoles.FirstOrDefaultAsync(gr => gr.Name == "User");
            if (defaultRole is not null)
            {
                await _dbContext.UsersGlobalRoles.AddAsync(new GlobalRoleUser { UserId = user.Id, RoleId = defaultRole.Id });
            }
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<string> LoginAsync(UserLoginPostDto model)
        {
            var dbUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (dbUser is null)
                return null;

            if (_passwordHasher.Verify(model.Password, dbUser.Salt, dbUser.PasswordHash) is false)
                return null;

            string token = await CreateTokenAsync(dbUser);

            return token;
        }

        private async Task<string> CreateTokenAsync(User user)
        {
            
        }
    }
}
