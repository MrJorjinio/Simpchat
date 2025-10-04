using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Token;
using System.Linq;
using System.Security.Claims;

namespace SimpchatWeb.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public UserController(
            SimpchatDbContext dbContext,
            IMapper mapper,
            ITokenService tokenService,
            IPasswordHasher passwordHasher
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpGet("{userId:guid}")]
        [EnsureEntityExistsFilter(typeof(User), "userId")]
        public IActionResult GetUserByUserId(
            Guid userId
            )
        {
            var user = _dbContext.Users.Find(userId);

            var response = _mapper.Map<UserProfileGetResponseDto>(user);
            return Ok(response);
        }

        [HttpGet("search/{username}")]
        public IActionResult SearchByUsername(string username)
        {
            var similarUsers = _dbContext.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{username}%"))
                .ToList();
            var response = _mapper.Map<ICollection<UserSearchResponseDto>>(similarUsers);

            return Ok(response);
        }

        [HttpPut("me")]
        [EnsureEntityExistsFilter(typeof(User), "userId")]
        public IActionResult UpdateMyProfile(
            UserPutDto request
            )
        {
            var userId = _tokenService.GetUserId(User);
            var dbUser = _dbContext.Users.Find(userId);

            dbUser = _mapper.Map(request, dbUser);
            _dbContext.SaveChanges();

            var response = _mapper.Map<UserResponseDto>(dbUser);

            return Ok(response);
        }

        [HttpPut("me/set-last-seen")]
        [EnsureEntityExistsFilter(typeof(User), "userId")]
        public IActionResult SetLastSeen()
        {
            var userId = _tokenService.GetUserId(User);
            var dbUser = _dbContext.Users.Find(userId);

            dbUser.LastSeen = DateTimeOffset.Now;
            _dbContext.SaveChanges();

            var response = _mapper.Map<UserSetLastSeenPutDto>(dbUser);

            return Ok(response);
        }

        [HttpPut("me/password")]
        [EnsureEntityExistsFilter(typeof(User), "userId")]
        public IActionResult UpdateMyPassword(
            UserPutPasswordDto request
            )
        {
            var userId = _tokenService.GetUserId(User);
            var dbUser = _dbContext.Users.Find(userId);

            if (_passwordHasher.Verify(request.CurrentPassword, dbUser.Salt, dbUser.PasswordHash) is false)
            {
                return BadRequest();
            }

            var newPasswordHash = _passwordHasher.Encrypt(request.NewPassword, dbUser.Salt);
            dbUser.PasswordHash = newPasswordHash;
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpDelete("me")]
        [EnsureEntityExistsFilter(typeof(User), "userId")]
        public IActionResult DeleteMe()
        {
            var userId = _tokenService.GetUserId(User);
            var dbUser = _dbContext.Users.Find(userId);

            _dbContext.Users.Remove(dbUser);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}
