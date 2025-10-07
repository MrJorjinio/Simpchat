using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Interfaces.Auth;
using System.Threading.Tasks;

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
        public async Task<IActionResult> RegisterAsync(
            UserRegisterPostDto request
            )
        {
            var user = await _authService.RegisterAsync(request);

            if (user is false)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            UserLoginPostDto request
            )
        {
            var token = await _authService.LoginAsync(request);

            if (token is null)
            {
                return BadRequest();
            }

            return Ok(token);
        }
    }
}
