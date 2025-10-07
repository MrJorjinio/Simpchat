using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Interfaces.Auth;
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
        public AuthService(
            SimpchatDbContext dbContext,
            IPasswordHasher passwordHasher,
            IOptions<AppSettings> options
            )
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _appSettings = options.Value;
        }

        public async Task<bool> RegisterAsync(UserRegisterPostDto user)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == user.Username))
            {
                return false;
            }

            string salt = Guid.NewGuid().ToString();
            string passwordHash = _passwordHasher.Encrypt(user.Password, salt);

            var _user = new User()
            {
                Username = user.Username,
                PasswordHash = passwordHash,
                Salt = salt,
                Description = string.Empty,
                ChatMemberAddPermissionType = ChatMemberAddPermissionType.WithConversations
            };

            await _dbContext.AddAsync(_user);
            await _dbContext.SaveChangesAsync();

            var defaultRole = await _dbContext.GlobalRoles.FirstOrDefaultAsync(gr => gr.Name == "User");
            if (defaultRole is not null)
            {
                await _dbContext.UsersGlobalRoles.AddAsync(new GlobalRoleUser { UserId = _user.Id, RoleId = defaultRole.Id });
            }
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<string> LoginAsync(UserLoginPostDto user)
        {
            var dbUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username);

            if (dbUser is null)
                return null;

            if (_passwordHasher.Verify(user.Password, dbUser.Salt, dbUser.PasswordHash) is false)
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
