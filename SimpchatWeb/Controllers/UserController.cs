using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Token;
using System.Security.Claims;

namespace SimpchatWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public UserController(SimpchatDbContext dbContext, IMapper mapper, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPut("info")]
        public IActionResult UpdateUser(UserUpdateDto request)
        {
            var userId = _tokenService.GetUserId(User);

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var dbUser = _dbContext.Users.Find(userId);

            if (dbUser is null)
            {
                return Unauthorized();
            }

            dbUser = _mapper.Map(request, dbUser);
            _dbContext.SaveChanges();

            var response = _mapper.Map<UserResponseDto>(dbUser);

            return Ok(response);
        }

        [HttpPut("password")]
        public IActionResult UpdatePassword(UserUpdatePasswordDto request)
        {
            var userId = _tokenService.GetUserId(User);

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var dbUser = _dbContext.Users.Find(userId);

            if (dbUser is null)
            {
                return Unauthorized();
            }

            if (_passwordHasher.Verify(request.CurrentPassword, dbUser.Salt, dbUser.PasswordHash) is false)
            {
                return BadRequest();
            }

            var newPasswordHash = _passwordHasher.Encrypt(request.NewPassword, dbUser.Salt);
            dbUser.PasswordHash = newPasswordHash;
            _dbContext.SaveChanges();

            var response = _mapper.Map<UserResponseDto>(dbUser);
            return Ok(response);
        }
    }
}
