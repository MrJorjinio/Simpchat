using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Features;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Users.Post;
using System.Security.AccessControl;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IValidator<PutChatDto> _updateValidator;
        private readonly IValidator<PostChatDto> _createValidator;
        private readonly IChatService _chatService;

        public GroupController(IGroupService groupService, IValidator<PutChatDto> updateValidator, IValidator<PostChatDto> createValidator, IChatService chatService)
        {
            _groupService = groupService;
            _updateValidator = updateValidator;
            _createValidator = createValidator;
            _chatService = chatService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync([FromForm]PostChatDto model, IFormFile? file)
        {
            var result = await _createValidator.ValidateAsync(model);

            if (!result.IsValid)
            {
                var errors = result.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new ValidationProblemDetails(errors));
            }

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

            model.OwnerId = userId;
            
            var response = await _groupService.CreateAsync(model, fileUploadRequest);

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

            var response = await _groupService.AddMemberAsync(chatId, userId);

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
        public async Task<IActionResult> UpdateAsync(Guid chatId, [FromForm]PutChatDto updateChatDto, IFormFile file)
        {
            var result = await _updateValidator.ValidateAsync(updateChatDto);

            if (!result.IsValid)
            {
                var errors = result.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new ValidationProblemDetails(errors));
            }

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

            var response = await _groupService.UpdateAsync(chatId, updateChatDto, fileUploadRequest);

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
