using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Temps;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Token;
using System.Collections.ObjectModel;

namespace SimpchatWeb.Controllers
{
    [Route("api/chats/{chatId}/messages")]
    [ApiController]
    public class ChatMessageController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SimpchatDbContext _dbContext;
        public ChatMessageController(
            ITokenService tokenService,
            IMapper mapper,
            SimpchatDbContext dbContext
            )
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpPut("{messageId:guid}/seen")]
        [EnsureEntityExistsFilter(typeof(Message), "messageId")]
        [EnsureEntityExistsFilter(typeof(User))]
        public IActionResult MarkAsSeen(Guid messageId)
        {
            var userId = _tokenService.GetUserId(User);
            var notification = _dbContext.Notifications
                .FirstOrDefault(n => n.MessageId == messageId && n.UserId == userId);

            if (notification is null)
            {
                return BadRequest();
            }

            notification.IsSeen = true;
            _dbContext.SaveChanges();

            var message = _dbContext.Messages.Find(messageId);

            var response = _mapper.Map<ChatMessageResponseDto>(message);
            response.IsSeen = notification.IsSeen;

            return Ok(response);
        }

        [HttpPost("conversation")]
        [EnsureEntityExistsFilter(typeof(User))]
        public IActionResult SendMessageToConversation(Guid? chatId, [FromBody] ChatMessageConversationPostDto request)
        {
            var userId = _tokenService.GetUserId(User);
            var receiver = _dbContext.Users.Find(request.ReceiverId);

            var chat = _dbContext.Chats
                .Include(c => c.Participants)
                .FirstOrDefault(c => c.Type == ChatType.Conversation &&
                                     c.Participants.Any(p => p.UserId == userId) &&
                                     c.Participants.Any(p => p.UserId == receiver.Id));

            if (chat is null)
            {
                chat = new Chat { Type = ChatType.Conversation };
                _dbContext.Chats.Add(chat);
                _dbContext.SaveChanges();

                _dbContext.ChatsParticipants.AddRange(
                    new ChatParticipant { ChatId = chat.Id, UserId = userId },
                    new ChatParticipant { ChatId = chat.Id, UserId = receiver.Id }
                );

                _dbContext.Conversations.Add(new Conversation { Id = chat.Id });
                _dbContext.SaveChanges();
            }

            var message = _mapper.Map<Message>(request);
            message.ChatId = chat.Id;
            message.SenderId = userId;

            if (_dbContext.Messages.Find(request.ReplyId) == null)
                message.ReplyId = null;

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            _dbContext.Notifications.Add(new Notification { MessageId = message.Id, UserId = receiver.Id });
            _dbContext.SaveChanges();

            var response = _mapper.Map<ChatMessageResponseDto>(message);
            return Ok(response);
        }


        [HttpPost]
        [EnsureEntityExistsFilter(typeof(User))]
        [EnsureEntityExistsFilter(typeof(ChatParticipant), "chatId")]
        [EnsureChatTypeNotFilter(ChatType.Conversation, "chatId")]
        [EnsureChatPermissionExistsFilter(ChatPermissionType.SendMessage, "chatId")]
        public IActionResult SendMessage(Guid chatId, [FromBody] ChatMessagePostDto request)
        {
            var userId = _tokenService.GetUserId(User);
            var user = _dbContext.Users.Find(userId);
            var chat = _dbContext.Chats.Find(chatId);

            var message = _mapper.Map<Message>(request);

            var replyId = request.ReplyId;
            if (replyId is not null)
            {
                var reply = _dbContext.Messages.Find(replyId);
                if (reply is not null)
                {
                    message.ReplyId = request.ReplyId;
                }
                else
                {
                    message.ReplyId = null;
                }
            }

            message.ChatId = chatId;
            message.SenderId = userId;
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            var receivers = _dbContext.ChatsParticipants
                .Where(cp => cp.ChatId == chat.Id);

            var notifications = new Collection<Notification>();

            foreach (var receiver in receivers)
            {
                var notification = new Notification { MessageId = message.Id, UserId = receiver.UserId };
                notifications.Add(notification);
            }

            _dbContext.AddRange(notifications);
            _dbContext.SaveChanges();

            var response = _mapper.Map<ChatMessageResponseDto>(message);
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetMessages(Guid chatId)
        {
            var chats = _dbContext.Chats
                .Where(c => c.Id == chatId)
                .Include(c => c.Messages)
                .ToList();
            var notfications = _dbContext.Notifications
                .ToList();

            var temp = new Collection<ChatMessageTempDto>();

            foreach (var chat in chats)
            {
                foreach (var message in chat.Messages)
                {
                    var dto = new ChatMessageTempDto
                    {
                        ChatId = chat.Id,
                        MessageId = message.Id,
                        Content = message.Content,
                        IsSeen = notfications.Any(n => n.MessageId == message.Id && n.IsSeen)
                    };

                    temp.Add(dto);
                }
            }
            var response = _mapper.Map<ICollection<ChatMessageResponseDto>>(temp);
            return Ok(response);
        }
    }
}
