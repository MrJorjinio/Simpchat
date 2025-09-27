using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos;
using SimpchatWeb.Services.Interfaces;
using SimpchatWeb.Services.Settings;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpchatWeb.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AppSettings _appSettings;
        public AuthService(SimpchatDbContext dbContext, IPasswordHasher passwordHasher, IOptions<AppSettings> options)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _appSettings = options.Value;
        }
        public UserResponseDto Register(UserRegisterDto user)
        {
            if (_dbContext.Users.Any(u => u.Username == user.Username))
            {
                return null;
            }
            string salt = Guid.NewGuid().ToString();
            string passwordHash = _passwordHasher.Encrypt(user.Password, salt);
            var _user = new User()
            {
                Username = user.Username,
                PasswordHash = passwordHash,
                Salt = salt,
                Description = string.Empty
            };
            _dbContext.Add(_user);
            _dbContext.SaveChanges();
            var dbUser = _dbContext.Users.FirstOrDefault(u => u.Username == user.Username);
            var defaultRole = _dbContext.GlobalRoles.FirstOrDefault(gr => gr.Name == "User");
            if (defaultRole is not null)
            {
                _user.GlobalRoles.Add(new GlobalRoleUser { UserId = dbUser.Id, RoleId = defaultRole.Id });
            }
            _dbContext.SaveChanges();
            var response = new UserResponseDto()
            {
                Username = user.Username
            };
            return response;
        }
        public string Login(UserLoginDto user)
        {
            var dbUser = _dbContext.Users
                .Include(u => u.GlobalRoles)
                    .ThenInclude(gr => gr.Role)
                    .FirstOrDefault(u => u.Username == user.Username);
            if (dbUser is null)
            {
                return null;
            }
            if (_passwordHasher.Verify(user.Password, dbUser.Salt, dbUser.PasswordHash) is false)
            {
                return null;
            }
            string token = CreateToken(dbUser);
            if (token is null)
            {
                return null;
            }
            return token;
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var roles = _dbContext.GlobalRolesUsers
                .Where(gru => gru.UserId == user.Id)
                .Include(gru => gru.Role)
                .Select(gru => gru.Role.Name)
                .ToList();
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
