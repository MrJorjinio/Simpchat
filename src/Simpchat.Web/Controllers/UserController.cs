using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Web.ViewModels.Users;

namespace Simpchat.Web.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var response = await _userService.GetByIdAsync(id);
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
        public async Task<IActionResult> SearchAsync(string username)
        {
            var users = await _userService.SearchByUsernameAsync(username);
            return Ok(users);
        }

        [HttpPut("last-seen/{userId}")]
        public async Task<IActionResult> SetLastSeenAsync(Guid userId)
        {
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

        [HttpPut("update-profile-pic/{userId}")]
        public async Task<IActionResult> UpdateProfilePicAsync(Guid userId, IFormFile profilePic)
        {
            var fileUploadRequest = new FileUploadRequest
            {
                FileName = profilePic.FileName,
                ContentType = profilePic.ContentType,
                Content = profilePic.OpenReadStream()
            };

            var response = await _userService.UpdateProfileAsync(userId, fileUploadRequest);
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
