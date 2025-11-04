using Simpchat.Application.Interfaces.Auth;
using Simpchat.Application.Interfaces.Email;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Users;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;
using System.Text.RegularExpressions;

namespace Simpchat.Application.Features
{
    public class AuthService : IAuthService
    {

        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IGlobalRoleRepository _globalRoleRepo;
        private readonly IGlobalRoleUserRepository _globalRoleUserRepository;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public AuthService(
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher passwordHasher,
            IUserRepository userRepo,
            IGlobalRoleRepository globalRoleRepo,
            IGlobalRoleUserRepository globalRoleUserRepo,
            IOtpService otpService,
            IEmailService emailService)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _userRepo = userRepo;
            _globalRoleRepo = globalRoleRepo;
            _globalRoleUserRepository = globalRoleUserRepo;
            _otpService = otpService;
            _emailService = emailService;
        }

        public async Task<ApiResult<string>> LoginAsync(LoginUserDto loginUserDto)
        {
            bool isEmail = Regex.IsMatch(loginUserDto.Credential, EmailRegex);

            var user = new User();

            if (isEmail is true)
            {
                user = await _userRepo.GetByEmailAsync(loginUserDto.Credential);

                if (user is null)
                {
                    return ApiResult<string>.FailureResult($"User with EMAIL[{loginUserDto.Credential}] not found", ResultStatus.NotFound);
                }

                if (await _passwordHasher.VerifyAsync(user.PasswordHash, loginUserDto.Password, user.Salt) is false)
                {
                    return ApiResult<string>.FailureResult("Email or Password is invalid", ResultStatus.Failure);
                }
            }
            else
            {
                user = await _userRepo.GetByUsernameAsync(loginUserDto.Credential);

                if (user is null)
                {
                    return ApiResult<string>.FailureResult($"User with USERNAME[{loginUserDto.Credential}] not found", ResultStatus.NotFound);
                }

                if (await _passwordHasher.VerifyAsync(user.PasswordHash, loginUserDto.Password, user.Salt) is false)
                {
                    return ApiResult<string>.FailureResult("Username or Password is invalid", ResultStatus.Failure);
                }
            }

            string jwtToken = await _jwtTokenGenerator.GenerateJwtTokenAsync(user.Id, await _globalRoleUserRepository.GetUserRolesAsync(user.Id));
            return ApiResult<string>.SuccessResult(jwtToken);
        }

        public async Task<ApiResult<Guid>> RegisterAsync(RegisterUserDto registerUserDto)
        {
            if (await _userRepo.GetByUsernameAsync(registerUserDto.Username) is not null)
            {
                return ApiResult<Guid>.FailureResult($"User with USERNAME[{registerUserDto.Username}] already exists", ResultStatus.Failure);
            }

            if (await _userRepo.GetByEmailAsync(registerUserDto.Email) is not null)
            {
                return ApiResult<Guid>.FailureResult($"User with EMAIL[{registerUserDto.Email}] already exists", ResultStatus.Failure);
            }

            var emailOtpCode = await _otpService.GetEmailOtpAsync(registerUserDto.Email);

            if (emailOtpCode != registerUserDto.OtpCode)
            {
                return ApiResult<Guid>.FailureResult($"OTP[{registerUserDto.OtpCode}] is wrong");
            }

            string salt = Guid.NewGuid().ToString();
            string passwordHash = await _passwordHasher.EncryptAsync(registerUserDto.Password, salt);

            var user = new User()
            {
                Username = registerUserDto.Username,
                Email = registerUserDto.Email,
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

        public async Task<ApiResult> ResetPasswordAsync(Guid userId, ResetPasswordDto resetPasswordDto)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            var userOtp = await _otpService.ValidateOtpCodeAsync(userId, resetPasswordDto.Otp);

            if (userOtp is null)
            {
                return ApiResult.FailureResult($"OTP[{resetPasswordDto.Otp}] is expired");
            }

            var newPasswrodHash = await _passwordHasher.EncryptAsync(resetPasswordDto.Password, user.Salt);
            user.PasswordHash = newPasswrodHash;

            await _userRepo.UpdateAsync(user);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> UpdatePasswordAsync(Guid userId, UpdatePasswordDto updatePasswordDto)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            if (await _passwordHasher.VerifyAsync(user.PasswordHash, updatePasswordDto.CurrentPassword, user.Salt) is false)
            {
                return ApiResult.FailureResult("Password is invalid", ResultStatus.Failure);
            }

            var newPasswrodHash = await _passwordHasher.EncryptAsync(updatePasswordDto.NewPassword, user.Salt);
            user.PasswordHash = newPasswrodHash;

            await _userRepo.UpdateAsync(user);

            return ApiResult.SuccessResult();
        }
    }
}
