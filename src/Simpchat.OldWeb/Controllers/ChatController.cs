using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Entity;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Entity;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpchatWeb.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(
            IChatService chatService
            )
        {
            _chatService = chatService;
        }

        [HttpGet("{chatId:guid}")]
        [EnsureChatPrivacyTypeNotFilter(ChatPrivacyType.Private)]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> GetChatByIdAsync(Guid chatId)
        {
            var currentUser = HttpContext.Items["RequestData/User"] as User;
            var response = await _chatService.GetChatByIdAsync(chatId, currentUser);
            return response;
        }

        [HttpGet("search/{name}")]
        public async Task<IActionResult> SearchChatsAsync(string name)
        {
            var response = await _chatService.SearchChatsAsync(name);
            return response;
        }

        [HttpPatch("{chatId:guid}/join")]
        [EnsureEntityExistsFilter(typeof(User))]
        [EnsureChatTypeNotFilter(ChatType.Conversation)]
        public async Task<IActionResult> JoinChatAsync(Guid chatId)
        {
            var currentUser = HttpContext.Items["RequestData/User"] as User;
            var chat = HttpContext.Items["RequestData/Chat"] as Chat;
            var response = await _chatService.JoinChatAsync(currentUser, chat);
            return response;
        }

        [HttpPatch("{chatId:guid}/update-privacy")]
        [EnsureEntityExistsFilter(typeof(ChatParticipant))]
        [EnsureChatPermissionExistsFilter(ChatPermissionType.ManageGroupBasics)]
        public async Task<IActionResult> UpdatePrivacyTypeAsync(Guid chatId, ChatPrivacyType chatPrivacyType)
        {
            var chat = HttpContext.Items["RequestData/Chat"] as Chat;
            var response = await _chatService.UpdatePrivacyTypeAsync(chat, chatPrivacyType);
            return response;
        }

        [HttpPost("{chatId:guid}/add-user/{userId:guid}")]
        [EnsureEntityExistsFilter(typeof(User), "token")]
        [EnsureEntityExistsFilter(typeof(User))]
        [EnsureEntityExistsFilter(typeof(Chat))]
        [EnsureChatTypeNotFilter(ChatType.Conversation)]
        [EnsureChatPermissionExistsFilter(ChatPermissionType.ManageUsers)]
        public async Task<IActionResult> AddUserToChatAsync(Guid chatId, Guid joiningUserId)
        {
            var fromUser = HttpContext.Items["RequestData/User"] as User;
            var response = await _chatService.AddUserToChatAsync(fromUser, chatId, joiningUserId);
            return response;
        }

        [HttpGet]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> GetMyChatsAsync()
        {
            var user = HttpContext.Items["RequestData/User"] as User;
            var response = await _chatService.GetMyChatsAsync(user);
            return response;
        }
    }
}