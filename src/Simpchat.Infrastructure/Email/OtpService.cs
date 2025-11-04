using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Email;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Domain.Entities;
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
        private readonly IUserOtpRepository _userOtpRepository;
        private readonly IEmailOtpRepository _emailOtpRepository;

        public OtpService(IUserRepository userRepo, IUserOtpRepository userOtpRepository, IEmailOtpRepository emailOtpRepository)
        {
            _userRepo = userRepo;
            _userOtpRepository = userOtpRepository;
            _emailOtpRepository = emailOtpRepository;
        }

        public async Task<string> GenerateAndSaveUserOtpAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                return "";

            var otpCode = new Random().Next(100000, 999999).ToString();

            var otp = new UserOtp
            {
                UserId = userId,
                Code = otpCode,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            await _userOtpRepository.CreateAsync(otp);

            return otpCode;
        }

        public async Task<string> GenerateAndSaveEmailOtpAsync(string email)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();

            var emailOtp = new EmailOtp
            {
                Code = otpCode,
                Email = email,
                ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            await _emailOtpRepository.CreateAsync(emailOtp);

            return otpCode;
        }

        public async Task<string> ValidateOtpCodeAsync(Guid userId, string code)
        {
            var otp = await _userOtpRepository.GetUserLatestAsync(userId);

            if (otp is not null)
            {
                if (otp.Code == code && otp.ExpiredAt > DateTimeOffset.UtcNow)
                {
                    return otp.Code;
                }
            }

            return "";
        }

        public async Task<string> GetEmailOtpAsync(string email)
        {
            var emailOtp = await _emailOtpRepository.GetLatestByEmailAsync(email);

            if (emailOtp is not null && emailOtp.ExpiredAt > DateTimeOffset.UtcNow)
            {
                return emailOtp.Code;
            }

            return "";
        }
    }
}
