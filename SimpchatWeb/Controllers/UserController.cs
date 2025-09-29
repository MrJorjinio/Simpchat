using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers()
        {
            var users = _dbContext.Users.ToList();
            var response = _mapper.Map<List<UserResponseDto>>(users);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUserById(Guid id)
        {
            var user = _dbContext.Users.Find(id);

            if (user is null)
            {
                return NotFound();
            }

            var response = _mapper.Map<UserResponseDto>(user);
            return Ok(response);
        }


        [HttpPut("me")]
        public IActionResult UpdateMyProfile(UserUpdateDto request)
        {
            var userId = _tokenService.GetUserId(User);

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var dbUser = _dbContext.Users.Find(userId);

            if (dbUser is null)
            {
                return NotFound();
            }

            dbUser = _mapper.Map(request, dbUser);
            _dbContext.SaveChanges();

            var response = _mapper.Map<UserResponseDto>(dbUser);

            return Ok(response);
        }

        [HttpPut("me/password")]
        public IActionResult UpdateMyPassword(UserUpdatePasswordDto request)
        {
            var userId = _tokenService.GetUserId(User);

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var dbUser = _dbContext.Users.Find(userId);

            if (dbUser is null)
            {
                return NotFound();
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
