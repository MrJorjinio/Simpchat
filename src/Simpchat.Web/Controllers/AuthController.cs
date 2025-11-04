using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Interfaces.Auth;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Users;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterUserDto> _registerValidator;
        private readonly IValidator<LoginUserDto> _loginUserDto;
        private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;
        private readonly IValidator<UpdatePasswordDto> _updatePasswordValidator;

        public AuthController(
            IAuthService authService,
            IValidator<RegisterUserDto> registerValidator,
            IValidator<LoginUserDto> loginUserDto,
            IValidator<ResetPasswordDto> resetPasswordValidator,
            IValidator<UpdatePasswordDto> updatePasswordValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginUserDto = loginUserDto;
            _resetPasswordValidator = resetPasswordValidator;
            _updatePasswordValidator = updatePasswordValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserDto registerUserDto)
        {
            await _registerValidator.ValidateAndThrowAsync(registerUserDto);

            var response = await _authService.RegisterAsync(registerUserDto);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginUserDto loginUserDto)
        {
            await _loginUserDto.ValidateAndThrowAsync(loginUserDto);

            var response = await _authService.LoginAsync(loginUserDto);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto)
        {
            await _updatePasswordValidator.ValidateAndThrowAsync(updatePasswordDto);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _authService.UpdatePasswordAsync(userId, updatePasswordDto);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPut("forgot-password")]
        [Authorize]
        public async Task<IActionResult> ForgotPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            await _resetPasswordValidator.ValidateAndThrowAsync(resetPasswordDto);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _authService.ResetPasswordAsync(userId, resetPasswordDto);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }
    }
}
