using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Interfaces.External.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Models.Chats.Get.ById;
using Simpchat.Application.Common.Models.Chats.Get.Profile;
using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Application.Common.Models.Chats.Post.Message;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Application.Common.Models.Users;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly SimpchatDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "chats-files";

        public ChatRepository(
            IUserRepository userRepository,
            IGroupRepository groupRepository,
            IChannelRepository channelRepository,
            SimpchatDbContext dbContext,
            IConversationRepository conversationRepository,
            IFileStorageService fileStorageService
            )
        {
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _channelRepository = channelRepository;
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task AddMessageAsync(MessagePostDto message, Guid currentUserId)
        {
            string? fileUrl = null;
            if (message.FileUploadRequest?.Content != null &&
                message.FileUploadRequest.FileName != null &&
                message.FileUploadRequest.ContentType != null)
            {
                fileUrl = await _fileStorageService.UploadFileAsync(
                    BucketName,
                    message.FileUploadRequest.FileName,
                    message.FileUploadRequest.Content,
                    message.FileUploadRequest.ContentType
                );
            }

            Guid chatId;
            if (message.ChatId != null)
            {
                chatId = message.ChatId.Value;
            }
            else
            {
                var receiverId = message.ReceiverId.Value;

                var existingConversation = await _dbContext.Conversations
                    .Include(c => c.User1)
                    .Include(c => c.User2)
                    .FirstOrDefaultAsync(c =>
                        (c.UserId1 == currentUserId && c.UserId2 == receiverId)
                        ||
                        (c.UserId2 == currentUserId && c.UserId1 == receiverId)
                    );

                if (existingConversation != null)
                {
                    chatId = existingConversation.Id;
                }
                else
                {
                    var newChat = new Chat
                    {
                        Id = Guid.NewGuid(),
                        Type = ChatType.Conversation,
                        CreatedAt = DateTime.UtcNow,
                    };

                    await _dbContext.SaveChangesAsync();

                    var newConversation = new Conversation
                    {
                        Id = newChat.Id,
                        UserId1 = currentUserId,
                        UserId2 = receiverId
                    };

                    var newMessage = new Message
                    {
                        ChatId = newChat.Id,
                        SenderId = currentUserId,
                        Content = message.Content,
                        FileUrl = fileUrl,
                        ReplyId = message.ReplyId,
                        SentAt = DateTime.UtcNow
                    };

                    _dbContext.Chats.Add(newChat);
                    _dbContext.Conversations.Add(newConversation);
                    _dbContext.Messages.Add(newMessage);

                    chatId = newChat.Id;
                    message.ChatId = newConversation.Id;
                }
            }

            if (message.ChatId != null)
            {
                var messageEntity = new Message
                {
                    ChatId = chatId,
                    SenderId = currentUserId,
                    Content = message.Content,
                    FileUrl = fileUrl,
                    ReplyId = message.ReplyId,
                    SentAt = DateTime.UtcNow
                };

                _dbContext.Messages.Add(messageEntity);
                await _dbContext.SaveChangesAsync();
            }
        }


        public async Task<ChatGetByIdDto> GetByIdAsync(Guid chatId, Guid currentUserId)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Channel)
                .Include(c => c.Group)
                .Include(c => c.Conversation)
                    .ThenInclude(conv => conv.User1)
                .Include(c => c.Conversation)
                    .ThenInclude(conv => conv.User2)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat is null)
            {
                return null;
            }

            string name = null;
            string avatarUrl = null;
            ChatType type;

            Guid? channelId = null;
            Guid? groupId = null;
            Guid? conversationId = null;

            if (chat.Channel != null)
            {
                type = ChatType.Channel;
                channelId = chat.Channel.Id;
                name = chat.Channel.Name;
                avatarUrl = chat.Channel.AvatarUrl;
            }
            else if (chat.Group != null)
            {
                type = ChatType.Group;
                groupId = chat.Group.Id;
                name = chat.Group.Name;
                avatarUrl = chat.Group.AvatarUrl;
            }
            else if (chat.Conversation != null)
            {
                type = ChatType.Conversation;
                conversationId = chat.Conversation.Id;

                var other = chat.Conversation.UserId1 == currentUserId ? chat.Conversation.User2 : chat.Conversation.User1;
                name = other?.Username ?? "Unknown";
                avatarUrl = other?.AvatarUrl;
            }
            else
            {
                type = ChatType.Group;
            }

            int participantsCount = 0;
            int participantsOnline = 0;
            if (channelId.HasValue)
            {
                participantsCount = await _dbContext.ChannelsSubscribers
                    .CountAsync(cs => cs.ChannelId == channelId.Value);
                participantsOnline = await _dbContext.ChannelsSubscribers
                    .CountAsync(cs => cs.ChannelId == channelId.Value && cs.User.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow);
            }
            else if (groupId.HasValue)
            {
                participantsCount = await _dbContext.GroupsMembers
                    .CountAsync(gm => gm.GroupId == groupId.Value);
                participantsOnline = await _dbContext.GroupsMembers
                    .CountAsync(cs => cs.GroupId == groupId.Value && cs.User.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow);
            }
            else if (conversationId.HasValue)
            {
                participantsCount = 2;
                participantsOnline = await _dbContext.Conversations
                    .CountAsync(cs => cs.Id == conversationId.Value && cs.User1.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow || cs.User2.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow);
            }

            var notificationsCount = await _dbContext.Notifications
                .Where(n => n.Message.ChatId == chatId && n.ReceiverId == currentUserId && !n.IsSeen)
                .CountAsync();

            var messagesQuery = _dbContext.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.SentAt)
                .ThenByDescending(m => m.Id)
                .Select(m => new
                {
                    m.Id,
                    m.Content,
                    m.FileUrl,
                    SenderId = m.SenderId,
                    SenderUsername = m.Sender.Username,
                    SenderAvatarUrl = m.Sender.AvatarUrl,
                    ReplyId = m.ReplyId,
                    m.SentAt,
                    IsSeen = _dbContext.Notifications.Any(n => n.MessageId == m.Id && n.IsSeen == true)
                })
                .AsNoTracking();

            var messagesRaw = await messagesQuery.ToListAsync();
            messagesRaw.Reverse();

            var messagesDto = messagesRaw.Select(m => new ChatGetByIdMessageDto
            {
                MessageId = m.Id,
                Content = m.Content,
                FileUrl = m.FileUrl,
                SenderId = m.SenderId,
                SenderUsername = m.SenderUsername,
                SenderAvatarUrl = m.SenderAvatarUrl,
                ReplyId = m.ReplyId,
                SentAt = m.SentAt,
                IsSeen = messagesRaw.Any(mr => mr.Id == m.Id && mr.IsSeen == true)
            }).ToList();

            var dto = new ChatGetByIdDto
            {
                Id = chat.Id,
                Name = name ?? string.Empty,
                AvatarUrl = avatarUrl,
                Type = type,
                ParticipantsCount = participantsCount,
                ParticipantsOnline = participantsOnline,
                NotificationsCount = notificationsCount,
                Messages = messagesDto
            };

            return dto;
        }

        public async Task<ChatGetByIdProfile> GetProfileByIdAsync(Guid chatId, Guid userId)
        {
            var chat = await _dbContext.Chats
                .Include(c => c.Channel)
                    .ThenInclude(c => c.Subscribers)
                        .ThenInclude(s => s.User)
                .Include(c => c.Group)
                    .ThenInclude(g => g.Members)
                        .ThenInclude(m => m.User)
                .Include(c => c.Conversation)
                    .ThenInclude(conv => conv.User1)
                .Include(c => c.Conversation)
                    .ThenInclude(conv => conv.User2)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat is null)
                return null;

            string? name = null;
            string? avatarUrl = null;
            string? description = null;
            var participantsDtos = new List<UserResponseDto>();
            int participantsCount = 0;
            int participantsOnlineCount = 0;

            if (chat.Type == ChatType.Conversation)
            {
                var oppositeUser = chat.Conversation.UserId1 == userId
                    ? chat.Conversation.User2
                    : chat.Conversation.User1;

                if (oppositeUser is null)
                    return null;

                var currentUser = await _userRepository.GetByIdAsync(userId);

                name = oppositeUser.Username;
                avatarUrl = oppositeUser.AvatarUrl;
                description = oppositeUser.Description;
                participantsCount = 2;

                var currentUserDto = new UserResponseDto
                {
                    Id = currentUser.Id,
                    Description = currentUser.Description,
                    AvatarUrl = currentUser.AvatarUrl,
                    IsOnline = currentUser.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow,
                    LastSeen = currentUser.LastSeen,
                    Username = currentUser.Username
                };

                var oppositeUserDto = new UserResponseDto
                {
                    Id = oppositeUser.Id,
                    Description = oppositeUser.Description,
                    AvatarUrl = oppositeUser.AvatarUrl,
                    IsOnline = oppositeUser.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow,
                    LastSeen = oppositeUser.LastSeen,
                    Username = oppositeUser.Username
                };

                participantsDtos.Add(currentUserDto);
                participantsDtos.Add(oppositeUserDto);
                participantsOnlineCount = participantsDtos.Count(u => u.IsOnline);
            }
            else if (chat.Type == ChatType.Group)
            {
                name = chat.Group.Name;
                description = chat.Group.Description;
                avatarUrl = chat.Group.AvatarUrl;
                var members = chat.Group.Members
                    .Select(m => new UserResponseDto
                    {
                        AvatarUrl = m.User.AvatarUrl,
                        Id = m.User.Id,
                        Description = m.User.Description,
                        IsOnline = m.User.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow,
                        LastSeen = m.User.LastSeen,
                        Username = m.User.Username
                    });

                participantsDtos.AddRange(members);

                participantsCount = members.Count();
                participantsCount = members.Count(p => p.IsOnline == true);
            }
            else if (chat.Type == ChatType.Channel)
            {
                name = chat.Channel.Name;
                description = chat.Channel.Description;
                avatarUrl = chat.Channel.AvatarUrl;

                var subscribers = chat.Channel.Subscribers
                    .Select(m => new UserResponseDto
                    {
                        AvatarUrl = m.User.AvatarUrl,
                        Id = m.User.Id,
                        Description = m.User.Description,
                        IsOnline = m.User.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow,
                        LastSeen = m.User.LastSeen,
                        Username = m.User.Username
                    });

                participantsDtos.AddRange(subscribers);

                participantsCount = subscribers.Count();
                participantsCount = subscribers.Count(p => p.IsOnline == true);
            }

            return new ChatGetByIdProfile
            {
                ChatId = chatId,
                Name = name,
                AvatarUrl = avatarUrl,
                Description = description,
                Participants = participantsDtos,
                ParticipantsCount = participantsCount,
                ParticipantsOnline = participantsOnlineCount
            };
        }

        public async Task<ICollection<UserChatResponseDto>?> GetUserChatsAsync(Guid userId)
        {
            var users = await _conversationRepository.GetUserConversationsAsync(userId);
            var groups = await _groupRepository.GetUserParticipatedGroupsAsync(userId);
            var channels = await _channelRepository.GetUserSubscribedChannelsAsync(userId);

            var result = users.Concat(groups).Concat(channels)
                .OrderByDescending(c => c.UserLastMessage ?? DateTimeOffset.MinValue)
                .ToList();

            return result;
        }

        public async Task<ICollection<ChatSearchResponseDto>?> SearchByNameAsync(string searchTerm, Guid currentUserId)
        {
            var users = await _userRepository.SearchByUsernameAsync(searchTerm, currentUserId);
            var groups = await _groupRepository.SearchByNameAsync(searchTerm);
            var channels = await _channelRepository.SearchByNameAsync(searchTerm);

            var result = users
                .Concat(groups)
                .Concat(channels)
                .ToList();

            return result;
        }

        public async Task UpdateAsync(Chat chat)
        {
            _dbContext.Update(chat);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Chat> GetByIdAsync(Guid chatId)
        {
            return await _dbContext.Chats
                .Include(c => c.Group)
                .Include(c => c.Channel)
                .Include(c => c.Conversation)
                .FirstOrDefaultAsync(c => c.Id == chatId);
        }
    }
}
