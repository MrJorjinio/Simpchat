using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Interfaces.Auth;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Users.Post;

namespace Simpchat.Web.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterUserDto> _validator;

        public AuthController(IAuthService authService, IValidator<RegisterUserDto> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var result = await _validator.ValidateAsync(registerUserDto);

            if (!result.IsValid)
            {
                var errors = result.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new ValidationProblemDetails(errors));
            }

            var response = await _authService.RegisterAsync(registerUserDto.Username, registerUserDto.Password);

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
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            var response = await _authService.LoginAsync(username, password);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPut("update-password/{userId}")]
        public async Task<IActionResult> UpdatePasswordAsync(Guid userId, string newPassword)
        {
            var response = await _authService.UpdatePasswordAsync(userId, newPassword);

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
