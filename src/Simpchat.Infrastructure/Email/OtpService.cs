using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Errors;
using Simpchat.Application.Interfaces.Email;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Domain.Entities;
using Simpchat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Email
{
    public class OtpService : IOtpService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        private readonly IUserOtpRepository _userOtpRepository;
        private readonly IEmailOtpRepository _emailOtpRepository;

        public OtpService(IUserRepository userRepo, IUserOtpRepository userOtpRepository, IEmailOtpRepository emailOtpRepository, IEmailService emailService)
        {
            _userRepo = userRepo;
            _userOtpRepository = userOtpRepository;
            _emailOtpRepository = emailOtpRepository;
            _emailService = emailService;
        }

        public async Task<Result<bool>> ValidateUserOtpAsync(Guid userId, string otpCode)
        {
            var otp = await _userOtpRepository.GetUserLatestAsync(userId);

            if (otp is not null)
            {
                if (otp.Code == otpCode && otp.ExpiredAt > DateTimeOffset.UtcNow)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return Result.Failure<bool>(ApplicationErrors.Otp.Expired);
        }

        public async Task<Result<bool>> ValidateEmailOtpAsync(string email, string otpCode)
        {
            var emailOtp = await _emailOtpRepository.GetLatestByEmailAsync(email);

            if (emailOtp is not null)
            {
                if (emailOtp.ExpiredAt > DateTimeOffset.UtcNow && emailOtp.Code == otpCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return Result.Failure<bool>(ApplicationErrors.Otp.Expired);
        }

        public async Task<Result> SendAndSaveUserOtpAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                return Result.Failure<Guid>(ApplicationErrors.User.IdNotFound);

            var otpCode = new Random().Next(100000, 999999).ToString();

            var otp = new UserOtp
            {
                UserId = userId,
                Code = otpCode,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            await _userOtpRepository.CreateAsync(otp);
            var response = await _emailService.SendOtpAsync(user.Email, otpCode);

            if (response.IsSuccess is false)
            {
                return Result.Failure(response.Error);
            }

            return Result.Success();
        }

        public async Task<Result> SendAndSaveEmailOtpAsync(string email)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();

            var emailOtp = new EmailOtp
            {
                Code = otpCode,
                Email = email,
                ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            await _emailOtpRepository.CreateAsync(emailOtp);
            var response = await _emailService.SendOtpAsync(email, otpCode);

            if (response.IsSuccess is false)
            {
                return Result.Failure(response.Error);
            }

            return Result.Success();
        }
    }
}
