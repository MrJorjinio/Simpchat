using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Common.Pagination.Chat;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Messages;
using Simpchat.Domain.Enums;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;
        private readonly IChatBanService _chatBanService;
        private readonly IValidator<PostMessageDto> _validator;

        public ChatController(IChatService chatService, IValidator<PostMessageDto> validator, IMessageService messageService, IChatBanService chatBanService)
        {
            _chatService = chatService;
            _validator = validator;
            _messageService = messageService;
            _chatBanService = chatBanService;
        }

        [HttpPost("search")]
        [Authorize]
        public async Task<IActionResult> SearchByNameAsync(ChatSearchPageModel model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _chatService.SearchAsync(model.searchTerm, userId);

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

            var response = await _chatService.AddUserPermissionAsync(userId, chatId, permissionName);

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
            var response = await _chatService.GetProfileAsync(chatId, userId);

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
            var response = await _chatService.UpdatePrivacyTypeAsync(chatId, privacyType);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPost("ban/{userId}")]
        public async Task<IActionResult> BanUserAsync(Guid chatId, Guid userId)
        {
            var response = await _chatBanService.BanUserAsync(chatId, userId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPost("unban/{userId}")]
        public async Task<IActionResult> UnbanUserAsync(Guid chatId, Guid userId)
        {
            var response = await _chatBanService.DeleteAsync(chatId, userId);

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
