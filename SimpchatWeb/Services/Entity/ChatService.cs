using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Entity;
using SimpchatWeb.Services.Interfaces.Minio;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SimpchatWeb.Services.Entity
{
    public class ChatService : IChatService
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "messages-files";

        public ChatService(
            SimpchatDbContext dbContext,
            IMapper mapper,
            ITokenService tokenService,
            IFileStorageService fileStorageService
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tokenService = tokenService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResult<UserJoinChatResponseDto>> AddUserToChatAsync(User fromUser, Guid chatId, Guid joiningUserId)
        {
            var joiningUser = _dbContext.Users.Find(joiningUserId);

            if (joiningUser is null)
                return new ApiResult<UserJoinChatResponseDto>(false, 404, "Joining user not found");

            if (joiningUser.ChatMemberAddPermissionType == ChatMemberAddPermissionType.Everyone)
            {
                var userChat = new ChatParticipant { UserId = joiningUser.Id, ChatId = chatId };
                await _dbContext.ChatsParticipants.AddAsync(userChat);
                await _dbContext.SaveChangesAsync();
            }
            else if (joiningUser.ChatMemberAddPermissionType == ChatMemberAddPermissionType.WithConversations)
            {
                var addingUserConversations = await _dbContext.ChatsParticipants
                    .Include(cp => cp.Chat)
                    .Where(cp => cp.UserId == joiningUser.Id && cp.Chat.Type == ChatType.Conversation)
                    .ToListAsync();

                var comingUserConversations = await _dbContext.ChatsParticipants
                    .Where(cp => cp.UserId == joiningUser.Id && cp.Chat.Type == ChatType.Conversation)
                    .ToListAsync();

                var isConversationBetweenExists = addingUserConversations.Any(auc => comingUserConversations.Any(cup => cup.ChatId == auc.ChatId));

                if (isConversationBetweenExists is true)
                {
                    var userChat = new ChatParticipant { UserId = joiningUser.Id, ChatId = chatId };
                    await _dbContext.ChatsParticipants.AddAsync(userChat);
                    await _dbContext.SaveChangesAsync();

                    var response = _mapper.Map<UserJoinChatResponseDto>(userChat);

                    return new ApiResult<UserJoinChatResponseDto>(true, 200, "Success", response);
                }
                else
                    return new ApiResult<UserJoinChatResponseDto>(false, 400, $"User already participated with chat");
            }
            return new ApiResult<UserJoinChatResponseDto>(false, 400, $"Joining user not accept any invitations");
        }

        public async Task<ApiResult<ChatGetByIdGetResponseDto>> GetChatByIdAsync(Guid chatId, User currentUser)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Group)
                    .ThenInclude(g => g.UserCreated)
                .Include(c => c.Channel)
                    .ThenInclude(c => c.UserCreated)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Sender)
                    .ThenInclude(m => m.Notifications)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
                return new ApiResult<ChatGetByIdGetResponseDto>(false, 404, $"Chat not found");

            if (chat.Type == ChatType.Conversation)
                return new ApiResult<ChatGetByIdGetResponseDto>(false, 406, $"Chat type is invalid");

            var response = new ChatGetByIdGetResponseDto
            {
                Id = chat.Id,
                Owner = chat.Type == ChatType.Channel
                ? chat.Channel.UserCreated.Username
                : chat.Type == ChatType.Group
                    ? chat.Group.UserCreated.Username
                    : "",
                Name = chat.Type == ChatType.Channel
                ? chat.Channel.Name
                : chat.Type == ChatType.Group
                    ? chat.Group.Name
                    : "",
                Description = chat.Type == ChatType.Channel
                ? chat.Channel.Description
                : chat.Type == ChatType.Group
                    ? chat.Group.Description
                    : "",
                MembersCount = chat.Participants.Count,
                MembersOnline = chat.Participants.Where(p => p.User.LastSeen.AddSeconds(10) > DateTimeOffset.UtcNow).Count(),
                Messages = chat.Messages.Select(m => new ChatMessageGetByIdGetResponseDto
                {
                    ChatId = m.ChatId,
                    MessageId = m.Id,
                    Content = m.Content,
                    SenderName = m.Sender.Username,
                    SentAt = m.SentAt,
                    IsSeen = m.Notifications.Where(n => n.IsSeen == true).Any()
                })
                .OrderBy(m => m.SentAt)
                .ToList(),
                ProfilePictureUrl = chat.Type == ChatType.Channel
                ? chat.Channel.ProfilePictureUrl
                : chat.Type == ChatType.Group
                    ? chat.Group.UserCreated.ProfilePictureUrl
                    : "",
            };

            return new ApiResult<ChatGetByIdGetResponseDto>(true, 200, "Success", response);
        }

        public async Task<ApiResult<ChatConversationGetByIdGetResponse>> GetConversationByIdAsync(Guid chatId, User currentUser)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Group)
                .Include(c => c.Channel)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
                return new ApiResult<ChatConversationGetByIdGetResponse>(false, 404, $"Chat not found");

            if (chat.Type == ChatType.Conversation)
            {
                var oppositeUser = chat.Participants.FirstOrDefault(p => p.UserId != currentUser.Id).User;
                var response = new ChatConversationGetByIdGetResponse
                {
                    Id = chat.Id,
                    Name = oppositeUser.Username,
                    IsOnline = oppositeUser.LastSeen.AddSeconds(10) > DateTimeOffset.UtcNow,
                    LastSeen = oppositeUser.LastSeen,
                    Messages = chat.Messages.Select(m => new ChatMessageGetByIdGetResponseDto
                    {
                        IsSeen = m.Notifications != null && m.Notifications.Any()
                    })
                    .ToList(),
                    ProfilePictureUrl = chat.Participants
                    .FirstOrDefault(p => p.UserId != currentUser.Id).User.ProfilePictureUrl
                 };
                return new ApiResult<ChatConversationGetByIdGetResponse>(true, 200, "Success", response);
            }

            return new ApiResult<ChatConversationGetByIdGetResponse>(false, 406, $"Chat type is invalid");
        }

        public async Task<ApiResult<IEnumerable<GetMyChatGetResponseDto>>> GetMyChatsAsync(User currentUser)
        {
            var chats = _dbContext.ChatsParticipants
                .Include(cp => cp.Chat)
                    .ThenInclude(c => c.Messages)
                        .ThenInclude(m => m.Sender)
                .Include(cp => cp.Chat.Messages)
                    .ThenInclude(m => m.Notifications)
                .Include(cp => cp.Chat.Participants)
                    .ThenInclude(cp => cp.User)
                .Include(cp => cp.Chat.Conversation)
                .Include(cp => cp.Chat.Group)
                .Include(cp => cp.Chat.Channel)
                .Where(cp => cp.UserId == currentUser.Id)
                .Select(cp => cp.Chat)
                .ToList();

            var orderedChats = chats
                .Select(c => new GetMyChatGetResponseDto
                {
                    Id = c.Id,
                    Type = c.Type,
                    Name = c.Type == ChatType.Conversation
                        ? c.Participants.FirstOrDefault(p => p.UserId != currentUser.Id)?.User?.Username ?? "Unknown User"
                        : c.Type == ChatType.Group
                            ? c.Group?.Name ?? "Unknown Group"
                            : c.Type == ChatType.Channel
                                ? c.Channel?.Name ?? "Unknown Channel"
                                : "Unknown Chat",
                    LastMessage = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => new ChatMessageGetByIdGetResponseDto
                        {
                            MessageId = m.Id,
                            Content = m.Content,
                            SentAt = m.SentAt,
                            SenderName = m.Sender?.Username ?? "Unknown Sender",
                            FileUrl = m.FileUrl
                        })
                        .FirstOrDefault(),
                    LastMessageTime = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.SentAt)
                        .FirstOrDefault(),
                    UnreadCount = c.Messages
                        .SelectMany(m => m.Notifications ?? new List<Notification>())
                        .Count(n => n.ReceiverId == currentUser.Id && !n.IsSeen),
                    ProfilePictureUrl = c.Type == ChatType.Channel
                        ? c.Channel?.ProfilePictureUrl ?? ""
                        : c.Type == ChatType.Group
                            ? c.Group?.ProfilePictureUrl ?? ""
                            : c.Type == ChatType.Conversation
                                ? c.Participants.FirstOrDefault(p => p.UserId != currentUser.Id)?.User?.ProfilePictureUrl ?? ""
                                : ""
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();

            return new ApiResult<IEnumerable<GetMyChatGetResponseDto>>(true, 200, "Success", orderedChats);
        }

        public async Task<ApiResult<UserJoinChatResponseDto>> JoinChatAsync(User currentUser, Chat chat)
        {
            var userChat = new ChatParticipant
            {
                UserId = currentUser.Id,
                ChatId = chat.Id,
            };

            await _dbContext.ChatsParticipants.AddAsync(userChat);
            await _dbContext.SaveChangesAsync();

            var userDefaultPermissions = new Collection<ChatUserPermission>();

            if (chat.Type == ChatType.Group)
            {
                var sendMessagePermission = await _dbContext.ChatPermissions.FirstOrDefaultAsync(cp => cp.Name == ChatPermissionType.SendMessage.ToString());

                if (sendMessagePermission is not null)
                {
                    userDefaultPermissions.Add(new ChatUserPermission { UserId = currentUser.Id, ChatId = chat.Id, PermissionId = sendMessagePermission.Id });
                }
            }

            var reactToMessagePermission = await _dbContext.ChatPermissions.FirstOrDefaultAsync(cp => cp.Name == ChatPermissionType.ReactToMessage.ToString());
            if (reactToMessagePermission is not null)
            {
                userDefaultPermissions.Add(new ChatUserPermission { UserId = currentUser.Id, ChatId = chat.Id, PermissionId = reactToMessagePermission.Id });
            }

            await _dbContext.AddRangeAsync(userDefaultPermissions);
            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<UserJoinChatResponseDto>(currentUser);
            response.ChatId = chat.Id;

            return new ApiResult<UserJoinChatResponseDto>(true, 200, "Success", response);
        }

        public async Task<ApiResult<IEnumerable<ChatSearchGetResponseDto>>> SearchChatsAsync(string name)
        {
            var results = new List<ChatSearchGetResponseDto>();

            var similarGroups = await _dbContext.Groups
                .Where(g => EF.Functions.Like(g.Name, $"%{name}%"))
                .ToListAsync();
            results.AddRange(similarGroups.Select(sg => new ChatSearchGetResponseDto
            {
                Id = sg.Id,
                Name = sg.Name,
                Type = ChatType.Group,
                ProfilePictureUrl = sg.ProfilePictureUrl
            }));

            var similarChannels = await _dbContext.Channels
                .Where(c => EF.Functions.Like(c.Name, $"%{name}%"))
                .ToListAsync();
            results.AddRange(similarChannels.Select(sc => new ChatSearchGetResponseDto
            {
                Id = sc.Id,
                Name = sc.Name,
                Type = ChatType.Channel,
                ProfilePictureUrl = sc.ProfilePictureUrl
            }));

            var similarUsers = await _dbContext.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{name}%"))
                .ToListAsync();
            results.AddRange(similarUsers.Select(su => new ChatSearchGetResponseDto
            {
                Id = su.Id,
                Name = su.Username,
                Type = ChatType.Conversation,
                ProfilePictureUrl = su.ProfilePictureUrl
            }));

            return new ApiResult<IEnumerable<ChatSearchGetResponseDto>>(true, 200, "Success", results);
        }

        public async Task<ApiResult<ChatMessageGetByIdGetResponseDto>> SendMessageAsync(User currentUser, Guid? chatId, ChatMessagePostDto model)
        {
            Chat chat = null;

            if (chatId.HasValue)
            {
                chat = await _dbContext.Chats
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Id == chatId.Value);
            }

            if (chat == null && model.ReceiverId != null)
            {
                var receiver = await _dbContext.Users.FindAsync(model.ReceiverId);
                if (receiver == null)
                    return new ApiResult<ChatMessageGetByIdGetResponseDto>(false, 404, "Receiver not found.");

                chat = await _dbContext.Chats
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Type == ChatType.Conversation &&
                                              c.Participants.Any(p => p.UserId == currentUser.Id) &&
                                              c.Participants.Any(p => p.UserId == receiver.Id));

                if (chat == null)
                {
                    chat = new Chat { Type = ChatType.Conversation };
                    await _dbContext.Chats.AddAsync(chat);
                    await _dbContext.SaveChangesAsync();

                    await _dbContext.ChatsParticipants.AddRangeAsync(
                        new ChatParticipant { ChatId = chat.Id, UserId = currentUser.Id },
                        new ChatParticipant { ChatId = chat.Id, UserId = receiver.Id }
                    );

                    await _dbContext.Conversations.AddAsync(new Conversation { Id = chat.Id });
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (chat == null)
                return new ApiResult<ChatMessageGetByIdGetResponseDto>(false, 400, "ChatId or ReceiverId must be provided.");

            var message = _mapper.Map<Message>(model);
            message.ChatId = chat.Id;
            message.SenderId = currentUser.Id;

            if (model.File != null && model.File.Length != 0)
            {
                var fileExtention = Path.GetExtension(model.File.FileName);
                var objectName = $"{Guid.NewGuid()}{fileExtention}";

                using (var stream = model.File.OpenReadStream())
                {
                    var fileUrl = await _fileStorageService.UploadFileAsync(BucketName, objectName, stream, fileExtention);
                    message.FileUrl = fileUrl;
                }
            }

            if (model.ReplyId.HasValue)
            {
                var replyMessage = await _dbContext.Messages.FindAsync(model.ReplyId);
                message.ReplyId = replyMessage?.Id;
            }

            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();

            var receivers = await _dbContext.ChatsParticipants
                .Where(cp => cp.ChatId == chat.Id && cp.UserId != currentUser.Id)
                .Select(cp => new Notification { MessageId = message.Id, ReceiverId = cp.UserId })
                .ToListAsync();

            await _dbContext.Notifications.AddRangeAsync(receivers);
            await _dbContext.SaveChangesAsync();

            var fullMessage = await _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Notifications)
                    .ThenInclude(n => n.Receiver)
                .FirstAsync(m => m.Id == message.Id);

            var response = _mapper.Map<ChatMessageGetByIdGetResponseDto>(fullMessage);
            response.IsSeen = fullMessage.Notifications.Where(n => n.IsSeen == false).Any();

            return new ApiResult<ChatMessageGetByIdGetResponseDto>(true, 200, "Success", response);
        }

        public async Task<ApiResult<ChatGetByIdGetResponseDto>> UpdatePrivacyTypeAsync(Chat chat, ChatPrivacyType chatPrivacyType)
        {
            chat.PrivacyType = chatPrivacyType;
            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<ChatGetByIdGetResponseDto>(chat);
            return new ApiResult<ChatGetByIdGetResponseDto>(true, 200, "Success", response);
        }
    }
}
