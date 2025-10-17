using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Chats.Post.Message;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Domain.Entities;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("search/{name}")]
        [Authorize]
        public async Task<IActionResult> SearchByNameAsync(string name)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _chatService.SearchByNameAsync(name, userId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyChatsAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _chatService.GetUserChatsAsync(userId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpGet("{chatId}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(Guid chatId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _chatService.GetByIdAsync(chatId, userId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpGet("{chatId}/profile")]
        [Authorize]
        public async Task<IActionResult> GetProfileByIdAsync(Guid chatId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _chatService.GetProfileByIdAsync(chatId, userId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessageAsync([FromForm]MessagePostApiRequestDto messagePostDto, IFormFile? file)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            FileUploadRequest? fileUploadRequest = null;

            if (file != null)
            {
                fileUploadRequest = new FileUploadRequest
                {
                    Content = file.OpenReadStream(),
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };
            }

            var messagePostRequest = new MessagePostDto
            {
                ChatId = messagePostDto.ChatId,
                Content = messagePostDto.Content,
                ReceiverId = messagePostDto.ReceiverId,
                ReplyId = messagePostDto.ReplyId,
                FileUploadRequest = fileUploadRequest
            };

            var response = await _chatService.SendMessageAsync(messagePostRequest, userId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPut("privacy-type")]
        public async Task<IActionResult> UpdatePrivacyTypeAsync(Guid chatId, ChatPrivacyType privacyType)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _chatService.UpdatePrivacyTypeAsync(chatId, userId, privacyType);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateAvatarAsync(Guid chatId, IFormFile file)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var newAvatarRequest = new FileUploadRequest
            {
                Content = file.OpenReadStream(),
                ContentType = file.ContentType,
                FileName = file.FileName
            };

            var response = await _chatService.UpdateAvatarAsync(chatId, userId, newAvatarRequest);

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
