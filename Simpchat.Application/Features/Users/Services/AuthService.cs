using Simpchat.Application.Common.Interfaces.Auth;
using Simpchat.Application.Common.Interfaces.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Users;
using Simpchat.Infrastructure.Identity;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Security
{
    internal class AuthService : IAuthService
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IGlobalRoleRepository _globalRoleRepository;

        public AuthService(
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            IGlobalRoleRepository globalRoleRepository)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _globalRoleRepository = globalRoleRepository;
        }

        public async Task<ApiResult<string>> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user is null || await _passwordHasher.VerifyAsync(user.PasswordHash, password, user.Salt) is false)
            {
                ApiResult<string>.FailureResult("Username or Password is invalid", ResultStatus.Failure);
            }

            string jwtToken = await _jwtTokenGenerator.GenerateJwtTokenAsync(user.Id, user.GlobalRoles.Select(r => r.Role));
            return ApiResult<string>.SuccessResult(jwtToken);
        }

        public async Task<ApiResult> RegisterAsync(string username, string password)
        {
            if (await _userRepository.GetByUsernameAsync(username) is not null)
            {
                return ApiResult.FailureResult($"User with Username[{username}] already exists", ResultStatus.Failure);
            }

            string salt = Guid.NewGuid().ToString();
            string passwordHash = await _passwordHasher.EncryptAsync(password, salt);

            var user = new User()
            {
                Username = username,
                PasswordHash = passwordHash,
                Salt = salt,
                Description = string.Empty,
                ChatMemberAddPermissionType = ChatMemberAddPermissionType.WithConversations
            };
            await _userRepository.AddAsync(user);

            var defaultRole = await _globalRoleRepository.GetByNameAsync("User");
            if (defaultRole is not null)
            {
                await _userRepository.AssignRoleAsync(user.Id, defaultRole.Id);
            }

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> UpdatePasswordAsync(Guid userId, string password)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            if (await _passwordHasher.VerifyAsync(user.PasswordHash, password, user.Salt) is false)
            {
                return ApiResult.FailureResult("Password is invalid", ResultStatus.Failure);
            }

            var newPasswrodHash = await _passwordHasher.EncryptAsync(password, user.Salt);
            user.PasswordHash = newPasswrodHash;

            await _userRepository.UpdateAsync(user);

            return ApiResult.SuccessResult();
        }
    }
}
