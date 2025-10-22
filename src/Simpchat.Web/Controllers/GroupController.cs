using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Files;
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
        public async Task<IActionResult> CreateAsync([FromForm]PostChatDto model, IFormFile? file)
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

        [HttpDelete("leave")]
        [Authorize]
        public async Task<IActionResult> LeaveAsync(Guid chatId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _groupService.DeleteMemberAsync(userId, chatId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(Guid chatId)
        {
            var response = await _groupService.DeleteAsync(chatId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateAsync(Guid chatId, PostChatDto updateChatDto)
        {
            var response = await _groupService.UpdateAsync(chatId, updateChatDto);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchAsync(string searchTerm)
        {
            var response = await _groupService.SearchAsync(searchTerm);

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
