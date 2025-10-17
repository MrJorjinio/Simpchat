using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Application.Common.Models.Users;
using Simpchat.Web.ViewModels.Users;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _userService.GetByIdAsync(id, userId);
            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpGet("search/{username}")]
        [Authorize]
        public async Task<IActionResult> SearchAsync(string username)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var users = await _userService.SearchByUsernameAsync(username, userId);
            return Ok(users);
        }

        [HttpPut("last-seen")]
        [Authorize]
        public async Task<IActionResult> SetLastSeenAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _userService.SetLastSeenAsync(userId);
            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPut("update-profile-pic")]
        [Authorize]
        public async Task<IActionResult> UpdateAvatarAsync(IFormFile profilePic)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var fileUploadRequest = new FileUploadRequest
            {
                FileName = profilePic.FileName,
                ContentType = profilePic.ContentType,
                Content = profilePic.OpenReadStream()
            };

            var response = await _userService.UpdateAvatarAsync(userId, fileUploadRequest);
            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPut("update-info")]
        public async Task<IActionResult> UpdateInfoAsync(UserUpdateInfoDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _userService.UpdateInfoAsync(userId, dto);

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
