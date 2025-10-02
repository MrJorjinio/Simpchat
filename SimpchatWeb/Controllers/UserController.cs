using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Token;
using System.Linq;
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

        [HttpGet("{username}")]
        public IActionResult GetUserById(
            string username
            )
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user is null)
            {
                return NotFound();
            }

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
        public IActionResult UpdateMyProfile(
            UserPutDto request
            )
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
        public IActionResult UpdateMyPassword(
            UserPutPasswordDto request
            )
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

            return Ok();
        }

        [HttpDelete("me")]
        public IActionResult DeleteMe()
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

            _dbContext.Users.Remove(dbUser);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPut("give-role")]
        public IActionResult GiveRoles(
            UserGlobalRolesPostDto request
            )
        {
            var user = _dbContext.Users
                .Include(u => u.GlobalRoles)
                .FirstOrDefault(u => u.Username == request.Username);

            if (user is null)
            {
                return BadRequest();
            }

            var dbGlobalRole = _dbContext.GlobalRoles
                .ToHashSet();

            var globalRoles = _mapper.Map<ICollection<GlobalRole>>(request.RoleNames);

            var existingRoles = dbGlobalRole
                .Where(dgr => globalRoles.Any(gr =>
                            string.Equals(
                                gr.Name, 
                                dgr.Name, 
                                StringComparison.OrdinalIgnoreCase
                                )));

            foreach (var existingRole in existingRoles)
            {
                var globalRoleUser = new GlobalRoleUser() { UserId = user.Id, RoleId = existingRole.Id };
                user.GlobalRoles.Add(globalRoleUser);
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
            }

            var response = _mapper.Map<UserResponseDto>(user);
            return Ok(response);
        }

        [HttpPut("{username}")]
        public IActionResult UpdateUser(
            string username, 
            UserPutDto request
            )
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user is null)
            {
                return BadRequest();
            }

            user = _mapper.Map(request, user);
            _dbContext.Update(user);
            _dbContext.SaveChanges();

            var response = _mapper.Map<UserResponseDto>(user);
            return Ok(response);
        }

        [HttpDelete("{userId:guid}")]
        public IActionResult DeleteUser(
            Guid userId
            )
        {
            var user = _dbContext.Users.Find(userId);

            if (user is null)
            {
                return BadRequest();
            }

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}
