using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos;
using SimpchatWeb.Services.Interfaces;

namespace SimpchatWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto request)
        {
            var user = _authService.Register(request);
            if (user is null)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPost("login")]
        public IActionResult Login(UserLoginDto request)
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
