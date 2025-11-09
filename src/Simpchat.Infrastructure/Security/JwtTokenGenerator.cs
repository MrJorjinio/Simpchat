using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Simpchat.Application.Interfaces.Auth;
using Simpchat.Domain.Entities;
using Simpchat.Shared.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Simpchat.Infrastructure.Security
{
    internal class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(IOptions<AppSettings> appSettings)
        {
            _jwtSettings = appSettings.Value.JwtSettings;
        }

        public async Task<string> GenerateJwtTokenAsync(Guid userId, GlobalRole role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role.Name.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
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
