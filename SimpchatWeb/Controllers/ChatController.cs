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
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Token;
using System.Collections.ObjectModel;

namespace SimpchatWeb.Controllers
{
    [Route("api/chats")]
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

        [HttpGet("{chatId:guid}")]
        [EnsureEntityExistsFilter(typeof(Chat), "chatId")]
        [EnsureChatPrivacyTypeNotFilter(ChatPrivacyType.Private)]
        public IActionResult GetChatById(Guid chatId)
        {
            var chat = _dbContext.Chats.Find(chatId);

            var response = _mapper.Map<ChatGetByIdGetResponseDto>(chat);
            var members = _dbContext.ChatsParticipants
                .Include(cp => cp.User)
                .Where(cp => cp.ChatId == chat.Id);
            var membersOnlineCount = members.Where(moc => moc.User.LastSeen.AddSeconds(4) > DateTimeOffset.UtcNow)
                .Count();
            response.MembersCount = members.Count();
            response.MembersOnline = membersOnlineCount;

            return Ok(response);
        }

        [HttpGet("search/{name}")]
        public IActionResult SearchChats(string name)
        {
            var results = new List<ChatSearchGetResponseDto>();

            var similarGroups = _dbContext.Groups
                .Where(g => EF.Functions.Like(g.Name, $"%{name}%"))
                .ToList();
            results.AddRange(similarGroups.Select(sg => new ChatSearchGetResponseDto
            {
                Id = sg.Id,
                Name = sg.Name,
                Type = ChatType.Group
            }));

            var similarChannels = _dbContext.Channels
                .Where(c => EF.Functions.Like(c.Name, $"%{name}%"))
                .ToList();
            results.AddRange(similarChannels.Select(sg => new ChatSearchGetResponseDto
            {
                Id = sg.Id,
                Name = sg.Name,
                Type = ChatType.Channel
            }));

            var similarUsers = _dbContext.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{name}%"))
                .ToList();
            results.AddRange(similarUsers.Select(sg => new ChatSearchGetResponseDto
            {
                Id = sg.Id,
                Name = sg.Username,
                Type = ChatType.Conversation
            }));

            return Ok(results);
        }

        [HttpPost("{chatId:guid}/join")]
        [EnsureEntityExistsFilter(typeof(User), "userId")]
        [EnsureChatTypeNotFilter(ChatType.Conversation, "chatId")]
        public IActionResult JoinChat(Guid chatId)
        {
            var userId = _tokenService.GetUserId(User);
            var user = _dbContext.Users.Find(userId);
            var chat = _dbContext.Chats.Find(chatId);

            var userChat = new ChatParticipant
            {
                UserId = user.Id,
                ChatId = chat.Id,
            };

            _dbContext.ChatsParticipants.Add(userChat);
            _dbContext.SaveChanges();

            var userDefaultPermissions = new Collection<ChatUserPermission>();

            if (chat.Type == ChatType.Group)
            {
                var sendMessagePermission = _dbContext.ChatPermissions.FirstOrDefault(cp => cp.Name == ChatPermissionType.SendMessage.ToString());

                if (sendMessagePermission is not null)
                {
                    userDefaultPermissions.Add(new ChatUserPermission { UserId = user.Id, ChatId = chat.Id, PermissionId = sendMessagePermission.Id });
                }
            }

            var reactToMessagePermission = _dbContext.ChatPermissions.FirstOrDefault(cp => cp.Name == ChatPermissionType.ReactToMessage.ToString());
            if (reactToMessagePermission is not null)
            {
                userDefaultPermissions.Add(new ChatUserPermission { UserId = user.Id, ChatId = chat.Id, PermissionId = reactToMessagePermission.Id });
            }

            _dbContext.AddRange(userDefaultPermissions);
            _dbContext.SaveChanges();

            var response = _mapper.Map<UserJoinChatResponseDto>(user);
            response.ChatId = chat.Id;
            return Ok(response);
        }

        [HttpPut("{chatId:guid}/update-privacy")]
        [EnsureEntityExistsFilter(typeof(User), "userId")]
        [EnsureEntityExistsFilter(typeof(Chat), "chatId")]
        [EnsureEntityExistsFilter(typeof(ChatParticipant), "chatId")]
        [EnsureChatPermissionExistsFilter(ChatPermissionType.ManageGroupBasics, "chatId")]
        public IActionResult UpdatePrivacyType(Guid chatId, ChatPrivacyTypePutResponseDto request)
        {
            var chat = _dbContext.Chats.Find(chatId);
            chat.PrivacyType = request.PrivacyType;
            _dbContext.SaveChanges();

            var response = _mapper.Map<ChatPrivacyTypePutResponseDto>(chat);
            return Ok(response);
        }

        [HttpPost("{chatId:guid}/add-user/{userId:guid}")]
        [EnsureEntityExistsFilter(typeof(User))]
        [EnsureEntityExistsFilter(typeof(User), "userId")]
        [EnsureEntityExistsFilter(typeof(Chat), "chatId")]
        [EnsureChatPermissionExistsFilter(ChatPermissionType.ManageUsers)]
        public IActionResult AddUserToChat(Guid chatId, Guid userId)
        {
            var addingUserId = _tokenService.GetUserId(User);
            var comingUser = _dbContext.Users.Find(userId);

            if (comingUser.ChatMemberAddPermissionType == ChatMemberAddPermissionType.Everyone)
            {
                var userChat = new ChatParticipant { UserId = userId, ChatId = chatId };
                _dbContext.ChatsParticipants.Add(userChat);
                _dbContext.SaveChanges();
            }
            else if (comingUser.ChatMemberAddPermissionType == ChatMemberAddPermissionType.WithConversations)
            {
                var addingUserConversations = _dbContext.ChatsParticipants
                    .Include(cp => cp.Chat)
                    .Where(cp => cp.UserId == addingUserId && cp.Chat.Type == ChatType.Conversation)
                    .ToList();
                var comingUserConversations = _dbContext.ChatsParticipants
                    .Where(cp => cp.UserId == addingUserId && cp.Chat.Type == ChatType.Conversation)
                    .ToList();
                var isConversationBetweenExists = addingUserConversations.Any(auc => comingUserConversations.Any(cup => cup.ChatId == auc.ChatId));

                if (isConversationBetweenExists is true)
                {
                    var userChat = new ChatParticipant { UserId = userId, ChatId = chatId };
                    _dbContext.ChatsParticipants.Add(userChat);
                    _dbContext.SaveChanges();
                }
                else
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [EnsureEntityExistsFilter(typeof(User))]
        public IActionResult GetMyChats()
        {
            var userId = _tokenService.GetUserId(User);

            var chats = _dbContext.ChatsParticipants
                .Include(cp => cp.Chat)
                    .ThenInclude(c => c.Messages)
                        .ThenInclude(m => m.Sender)
                .Include(cp => cp.Chat.Messages)
                    .ThenInclude(m => m.Notifications)
                .Include(cp => cp.Chat.Participants)
                    .ThenInclude(cp => cp.User)
                .Include(cp => cp.Chat.Group)
                .Include(cp => cp.Chat.Channel)
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.Chat)
                .ToList();

            var orderedChats = chats
                .Select(c => new
                {
                    ChatId = c.Id,
                    ChatType = c.Type.ToString(),

                    ChatName = c.Type == ChatType.Conversation
                        ? c.Participants
                            .Where(cp => cp.UserId != userId)
                            .Select(cp => cp.User.Username)
                            .FirstOrDefault()
                        : c.Type == ChatType.Group
                        ? c.Group.Name
                        : c.Type == ChatType.Channel
                        ? c.Channel.Name
                        : "Unknown Chat",

                    LastMessage = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => new
                        {
                            m.Id,
                            m.Content,
                            m.SentAt,
                            Sender = m.Sender.Username
                        })
                        .FirstOrDefault(),

                    LastMessageTime = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.SentAt)
                        .FirstOrDefault(),

                    UnreadCount = c.Messages
                        .SelectMany(m => m.Notifications)
                        .Count(n => n.UserId == userId && !n.IsSeen)
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();

            return Ok(orderedChats);
        }
    }
}
