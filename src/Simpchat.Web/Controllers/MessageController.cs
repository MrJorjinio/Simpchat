using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Messages;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMessageReactionService _messageReactionService;
        private readonly IValidator<PostMessageDto> _postMessageValidator;
        private readonly IValidator<UpdateMessageDto> _updateMessageValidator;

        public MessageController(IMessageService messageService, IMessageReactionService messageReactionService, IValidator<PostMessageDto> postMessageValidator, IValidator<UpdateMessageDto> updateMessageValidator)
        {
            _messageService = messageService;
            _messageReactionService = messageReactionService;
            _postMessageValidator = postMessageValidator;
            _updateMessageValidator = updateMessageValidator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessageAsync([FromForm] PostMessageDto messagePostDto, IFormFile? file)
        {
            var result = await _postMessageValidator.ValidateAsync(messagePostDto);

            if (!result.IsValid)
            {
                var errors = result.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new ValidationProblemDetails(errors));
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            UploadFileRequest? fileUploadRequest = null;

            if (file != null)
            {
                fileUploadRequest = new UploadFileRequest
                {
                    Content = file.OpenReadStream(),
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };
            }

            var messagePostRequest = new PostMessageDto
            {
                ChatId = messagePostDto.ChatId,
                Content = messagePostDto.Content,
                ReceiverId = messagePostDto.ReceiverId,
                ReplyId = messagePostDto.ReplyId,
                SenderId = userId
            };

            var response = await _messageService.SendMessageAsync(messagePostRequest, fileUploadRequest);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPut("{messageId}")]
        public async Task<IActionResult> UpdateAsync(Guid messageId, [FromForm] UpdateMessageDto updateMessageDto, IFormFile? file)
        {
            await _updateMessageValidator.ValidateAndThrowAsync(updateMessageDto);

            UploadFileRequest? fileUploadRequest = null;

            if (file != null)
            {
                fileUploadRequest = new UploadFileRequest
                {
                    Content = file.OpenReadStream(),
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };
            }

            var response = await _messageService.UpdateAsync(messageId, updateMessageDto, fileUploadRequest);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteAsync(Guid messageId)
        {
            var response = await _messageService.DeleteAsync(messageId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpPost("reaction")]
        [Authorize]
        public async Task<IActionResult> PutReactionAsync(Guid messageId, Guid reactionId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _messageReactionService.CreateAsync(messageId, reactionId, userId);

            return response.Status switch
            {
                ResultStatus.Success => Ok(response),
                ResultStatus.NotFound => NotFound(response),
                ResultStatus.Failure => BadRequest(response),
                ResultStatus.Unauthorized => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        [HttpDelete("reaction")]
        [Authorize]
        public async Task<IActionResult> DeleteReactionAsync(Guid messageId, Guid reactionId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _messageReactionService.DeleteAsync(messageId, userId);

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
