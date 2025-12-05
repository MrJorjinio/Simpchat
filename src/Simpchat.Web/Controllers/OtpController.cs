using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.Email;
using Simpchat.Application.Models.ApiResult;

using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/otp")]
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
        public async Task<IActionResult> SendToUserAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _otpService.SendAndSaveUserOtpAsync(userId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpPost("send-to-email")]
        public async Task<IActionResult> SendToEmailAsync(string email)
        {
            var response = await _otpService.SendAndSaveEmailOtpAsync(email);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }
    }
}
