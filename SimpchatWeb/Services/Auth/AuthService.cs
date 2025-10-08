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
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _dbContext.UsersGlobalRoles
                .Where(gru => gru.UserId == user.Id)
                .Include(gru => gru.Role)
                .Select(gru => gru.Role.Name)
                .ToListAsync();

            if (roles is not null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_appSettings.JwtSettings.Key)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _appSettings.JwtSettings.Issuer,
                audience: _appSettings.JwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            var token = new JwtSecurityTokenHandler()
                .WriteToken(tokenDescriptor);

            return token;
        }
    }
}
