using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Interfaces.Email;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;

        public OtpController(IEmailService emailService, IOtpService otpService)
        {
            _emailService = emailService;
            _otpService = otpService;
        }

        [HttpPost("send-to-user")]
        [Authorize]
        public async Task<IActionResult> SendToUserAsync(string email)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var otp = await _otpService.GenerateAndSaveUserOtpAsync(userId);
            await _emailService.SendOtpAsync(email, otp);

            return Ok(ApiResult.SuccessResult());
        }

        [HttpPost("send-to-email")]
        public async Task<IActionResult> SendToEmailAsync(string email)
        {
            var otpCode = await _otpService.GenerateAndSaveEmailOtpAsync(email);
            await _emailService.SendOtpAsync(email, otpCode);

            return Ok(ApiResult.SuccessResult());
        }
    }
}
