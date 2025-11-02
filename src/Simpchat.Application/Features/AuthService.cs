using Simpchat.Application.Interfaces.Auth;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Infrastructure.Identity;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features
{
    public class AuthService : IAuthService
    {

        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly INewUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IGlobalRoleRepository _globalRoleRepo;
        private readonly IGlobalRoleUserRepository _globalRoleUserRepository;

        public AuthService(
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher passwordHasher,
            INewUserRepository userRepo,
            IGlobalRoleRepository globalRoleRepo,
            IGlobalRoleUserRepository globalRoleUserRepo)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _userRepo = userRepo;
            _globalRoleRepo = globalRoleRepo;
            _globalRoleUserRepository = globalRoleUserRepo;
        }

        public async Task<ApiResult<string>> LoginAsync(string username, string password)
        {
            var user = await _userRepo.GetByUsernameAsync(username);

            if (user is null || await _passwordHasher.VerifyAsync(user.PasswordHash, password, user.Salt) is false)
            {
                return ApiResult<string>.FailureResult("Username or Password is invalid", ResultStatus.Failure);
            }

            string jwtToken = await _jwtTokenGenerator.GenerateJwtTokenAsync(user.Id, await _globalRoleUserRepository.GetUserRolesAsync(user.Id));
            return ApiResult<string>.SuccessResult(jwtToken);
        }

        public async Task<ApiResult<Guid>> RegisterAsync(string username, string password)
        {
            if (await _userRepo.GetByUsernameAsync(username) is not null)
            {
                return ApiResult<Guid>.FailureResult($"User with Username[{username}] already exists", ResultStatus.Failure);
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
            await _userRepo.CreateAsync(user);

            var defaultRole = await _globalRoleRepo.GetByNameAsync("User");

            if (defaultRole is not null)
            {
                var globalRoleUser = new GlobalRoleUser
                {
                    RoleId = defaultRole.Id,
                    UserId = user.Id
                };

                await _globalRoleUserRepository.CreateAsync(globalRoleUser);
            }

            return ApiResult<Guid>.SuccessResult(user.Id);
        }

        public async Task<ApiResult> UpdatePasswordAsync(Guid userId, string password)
        {
            var user = await _userRepo.GetByIdAsync(userId);

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

            await _userRepo.UpdateAsync(user);

            return ApiResult.SuccessResult();
        }
    }
}
