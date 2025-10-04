using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Interfaces.Auth;

namespace SimpchatWeb.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(
            IAuthService authService
            )
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(
            UserRegisterPostDto request
            )
        {
            var user = _authService.Register(request);

            if (user is false)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(
            UserLoginPostDto request
            )
        {
            var token = _authService.Login(request);

            if (token is null)
            {
                return BadRequest();
            }

            return Ok(token);
        }
    }
}
