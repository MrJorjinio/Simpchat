using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Temps;
using SimpchatWeb.Services.Interfaces.Token;
using System.Collections.ObjectModel;

namespace SimpchatWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public ChatController(
            SimpchatDbContext dbContext,
            IMapper mapper,
            ITokenService tokenService
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpGet("{chatId:guid}/messages")]
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

        [HttpPost("{chatId:guid}/send-message")]
        public IActionResult SendMessage(Guid chatId, [FromBody]ChatMessagePostDto request)
        {
            var chat = _dbContext.Chats.Find(chatId);

            if (chat is null)
            {
                return BadRequest();
            }

            if (chat.Type == ChatTypes.Conversation)
            {
                return BadRequest();
            }

            var userId = _tokenService.GetUserId(User);

            if (userId == Guid.Empty)
            {
                return BadRequest();
            }
            var user = _dbContext.Users.Find(userId);

            if (user is null)
            {
                return BadRequest();
            }

            var message = _mapper.Map<Message>(request);

            var replyId = request.ReplyId;
            if (replyId is not null)
            {
                var reply = _dbContext.Messages.Find(replyId);
                if (reply is not null)
                {
                    message.ReplyId = null;
                }
            }

            message.ChatId = chatId;
            message.SenderId = userId;
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            var notification = new Notification { MessageId = message.Id, UserId = user.Id };
            _dbContext.Notifications.Add(notification);
            _dbContext.SaveChanges();

            var response = _mapper.Map<ChatMessageResponseDto>(message);
            return Ok(response);
        }

        [HttpPost("send-message/conversation")]
        public IActionResult SendMessageToConversation(Guid chatId, [FromBody]ChatMessageConversationPostDto request)
        {
            var userId = _tokenService.GetUserId(User);

            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var user = _dbContext.Users.Find(userId);

            if (user is null)
            {
                return BadRequest();
            }

            var chat = _dbContext.Chats.Find(chatId);

            if (chat is null)
            {
                chat = new Chat { Type = ChatTypes.Conversation };
                _dbContext.Chats.Add(chat);
                _dbContext.SaveChanges();
                var chatParticipant = new ChatParticipant { ChatId = chat.Id, UserId = userId };

                var withUser = _dbContext.Users.Find(request.ReceiverId);

                if (withUser is null)
                {
                    return BadRequest();
                }

                var chatParticipantWith = new ChatParticipant { ChatId = chat.Id, UserId = withUser.Id };

                _dbContext.ChatsParticipants.Add(chatParticipant);
                _dbContext.ChatsParticipants.Add(chatParticipantWith);
                _dbContext.SaveChanges();

                var conversation = new Conversation { Id = chat.Id };
                _dbContext.Conversations.Add(conversation);
                _dbContext.SaveChanges();
            }
            else
            {
                if (chat.Type != ChatTypes.Conversation)
                {
                    return BadRequest();
                }
            }

            var message = _mapper.Map<Message>(request);

            var replyId = request.ReplyId;
            if (replyId is not null)
            {
                var reply = _dbContext.Messages.Find(replyId);
                if (reply is not null)
                {
                    message.ReplyId = null;
                }
            }

            message.ChatId = chat.Id;
            message.SenderId = userId;
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            var notification = new Notification { MessageId = message.Id, UserId = user.Id };
            _dbContext.Notifications.Add(notification);
            _dbContext.SaveChanges();

            var response = _mapper.Map<ChatMessageResponseDto>(message);
            return Ok(response);
        }

        [HttpPut("mark-as-seen/{messageId:guid}")]
        public IActionResult MarkAsSeen(Guid messageId)
        {
            var notification = _dbContext.Notifications
                .FirstOrDefault(n => n.MessageId == messageId);

            if (notification is null)
            {
                return BadRequest();
            }

            notification.IsSeen = true;
            _dbContext.SaveChanges();

            var message = _dbContext.Messages.Find(messageId);

            if (message is null)
            {
                return BadRequest();
            }

            var response = _mapper.Map<ChatMessageResponseDto>(message);
            response.IsSeen = notification.IsSeen;

            return Ok(response);
        }
    }
}
