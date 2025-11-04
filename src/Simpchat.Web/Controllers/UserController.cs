using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Users;
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

            var users = await _userService.SearchAsync(username, userId);
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

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromForm]UpdateUserDto model, IFormFile? file)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var fileUploadRequest = new UploadFileRequest();

            if (file is not null)
            {
                fileUploadRequest = new UploadFileRequest
                {
                    Content = file.OpenReadStream(),
                    ContentType = file.ContentType,
                    FileName = file.Name
                };
            }

            var response = await _userService.UpdateAsync(userId, model, fileUploadRequest);

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
