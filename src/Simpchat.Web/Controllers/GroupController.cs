using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Chats.Post;
using Simpchat.Application.Common.Models.Files;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync([FromForm]ChatPostDto model, IFormFile? file)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var fileUploadRequest = new FileUploadRequest();

            if (file is not null)
            {
                fileUploadRequest = new FileUploadRequest
                {
                    Content = file.OpenReadStream(),
                    ContentType = file.ContentType,
                    FileName = file.Name
                };
            }
            
            var response = await _groupService.CreateAsync(userId, model, fileUploadRequest);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPost("add-member")]
        [Authorize]
        public async Task<IActionResult> AddMemberAsync(Guid chatId, Guid addingUserId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _groupService.AddUserAsync(chatId, addingUserId, userId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPost("add-user-permission")]
        [Authorize]
        public async Task<IActionResult> AddPermissionAsync(string permissionName, Guid chatId, Guid addingUserId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _groupService.AddUserPermissionAsync(permissionName, chatId, addingUserId, userId);

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
