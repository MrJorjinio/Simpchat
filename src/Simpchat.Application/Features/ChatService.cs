using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Users;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;

namespace Simpchat.Application.Features
{
    internal class ChatService : IChatService
    {
        private readonly IChatRepository _repo;
        private readonly IMessageRepository _messageRepo;
        private readonly IConversationRepository _conversationRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IChannelRepository _channelRepo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly IGroupService _groupService;
        private readonly IChannelService _channelService;
        private readonly IConversationService _conversationService;
        private readonly IUserService _userService;
        private readonly IChatPermissionRepository _chatPermissionRepository;
        private readonly IChatUserPermissionRepository _chatUserPermissionRepository;

        public ChatService(
            IChatRepository chatRepository,
            IMessageRepository messageRepo,
            IConversationRepository conversationRepo,
            IGroupRepository groupRepo,
            IChannelRepository channelRepo,
            IUserRepository userRepo,
            INotificationRepository notificationRepo,
            IFileStorageService fileStorageService,
            IChannelService channelService,
            IConversationService conversationService,
            IUserService userService,
            IChatPermissionRepository chatPermissionRepository,
            IChatUserPermissionRepository chatUserPermissionRepository,
            IGroupService groupService)
        {
            _repo = chatRepository;
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _groupRepo = groupRepo;
            _channelRepo = channelRepo;
            _userRepo = userRepo;
            _notificationRepo = notificationRepo;
            _fileStorageService = fileStorageService;
            _channelService = channelService;
            _conversationService = conversationService;
            _userService = userService;
            _chatPermissionRepository = chatPermissionRepository;
            _chatUserPermissionRepository = chatUserPermissionRepository;
            _groupService = groupService;
        }

        public async Task<ApiResult<Guid>> AddUserPermissionAsync(Guid chatId, Guid userId, string permissionName)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<Guid>.FailureResult($"User with ID{userId} not found");
            }

            var chatPermission = await _chatPermissionRepository.GetByNameAsync(permissionName);

            if (chatPermission is null)
            {
                return ApiResult<Guid>.FailureResult($"Chat-Permission with NAME[{permissionName}] not found");
            }

            var chatUserPermission = new ChatUserPermission
            {
                ChatId = chatId,
                PermissionId = chatPermission.Id,
                UserId = userId
            };

            await _chatUserPermissionRepository.CreateAsync(chatUserPermission);

            return ApiResult<Guid>.SuccessResult(chatUserPermission.Id);
        }

        public async Task<ApiResult<GetByIdChatDto>> GetByIdAsync(Guid chatId, Guid userId)
        {
            var chat = await _repo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult<GetByIdChatDto>.FailureResult($"Chat with ID[{chatId}] not found");
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<GetByIdChatDto>.FailureResult($"User with ID{userId} not found");
            }

            var participantsCount = 0;
            var participantsOnline = 0;
            var avatarUrl = "";
            var name = "";

            if (chat.Type == ChatType.Conversation)
            {
                var conversation = await _conversationRepo.GetByIdAsync(chatId);

                var isParticipated = conversation.UserId1 == userId || conversation.UserId2 == userId;

                if (isParticipated is false)
                {
                    return ApiResult<GetByIdChatDto>.FailureResult($"User don't participated in chat[{chatId}]", ResultStatus.Unauthorized);
                }

                participantsCount = 2;

                if (conversation.User1.LastSeen.GetOnlineStatus() is true)
                {
                    participantsOnline++;
                }

                if (conversation.User2.LastSeen.GetOnlineStatus() is true)
                {
                    participantsOnline++;
                }

                avatarUrl = conversation.UserId1 == userId ? conversation.User2.AvatarUrl : conversation.User1.AvatarUrl;
                name = conversation.UserId1 == userId ? conversation.User2.Username : conversation.User1.Username;
            }
            else if (chat.Type == ChatType.Group)
            {
                var group = await _groupRepo.GetByIdAsync(chatId);

                bool isParticipated = group.Members.FirstOrDefault(m => m.UserId == userId) != null;

                if (isParticipated is not true)
                {
                    return ApiResult<GetByIdChatDto>.FailureResult($"User don't participated in chat[{chatId}]", ResultStatus.Unauthorized);
                }

                participantsCount = group.Members.Count();

                foreach (var groupMember in group.Members)
                {
                    if (groupMember.User.LastSeen.GetOnlineStatus() is true)
                    {
                        participantsOnline++;
                    }
                }

                avatarUrl = group.AvatarUrl;
                name = group.Name;
            }
            else if (chat.Type == ChatType.Channel)
            {
                var channel = await _channelRepo.GetByIdAsync(chatId);

                bool isParticipated = channel.Subscribers.FirstOrDefault(m => m.UserId == userId) != null;

                if (isParticipated is not true)
                {
                    return ApiResult<GetByIdChatDto>.FailureResult($"User don't participated in chat[{chatId}]", ResultStatus.Unauthorized);
                }

                participantsCount = channel.Subscribers.Count();

                foreach (var channelSubscriber in channel.Subscribers)
                {
                    if (channelSubscriber.User.LastSeen.GetOnlineStatus() is true)
                    {
                        participantsOnline++;
                    }
                }

                avatarUrl = channel.AvatarUrl;
                name = channel.Name;
            }


            var messagesModels = new List<GetByIdMessageDto>();

            foreach (var message in chat.Messages)
            {
                var messageModel = new GetByIdMessageDto
                {
                    MessageId = message.Id,
                    Content = message.Content,
                    FileUrl = message.Content,
                    ReplyId = message.ReplyId,
                    IsSeen = (await _notificationRepo.GetMessageSeenStatusAsync(message.Id)),
                    SenderAvatarUrl = message.Sender.AvatarUrl,
                    SenderUsername = message.Sender.Username,
                    SenderId = message.SenderId,
                    SentAt = message.SentAt,
                    IsNotificated = await _notificationRepo.CheckIsNotSeenAsync(message.Id, userId),
                    NotificationId = await _notificationRepo.GetIdAsync(message.Id, userId)
                };

                messagesModels.Add(messageModel);
            }

            var notificationsCount = await _notificationRepo.GetUserChatNotificationsCountAsync(userId, chatId);

            var model = new GetByIdChatDto
            {
                Id = chat.Id,
                AvatarUrl = avatarUrl,
                Messages = messagesModels,
                Name = name,
                NotificationsCount = notificationsCount,
                ParticipantsCount = participantsCount,
                ParticipantsOnline = participantsOnline,
                Type = chat.Type
            };

            return ApiResult<GetByIdChatDto>.SuccessResult(model);
        }

        public async Task<ApiResult<GetByIdChatProfile>> GetProfileAsync(Guid chatId, Guid userId)
        {
            var chat = await _repo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult<GetByIdChatProfile>.FailureResult($"Chat with ID[{chatId}] not found");
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<GetByIdChatProfile>.FailureResult($"User with ID{userId} not found");
            }

            var participantsCount = 0;
            var participantsOnline = 0;
            var participants = new List<UserResponseDto>();
            var avatarUrl = "";
            var name = "";
            var description = "";

            if (chat.Type == ChatType.Conversation)
            {
                var conversation = await _conversationRepo.GetByIdAsync(chatId);

                var isParticipated = conversation.UserId1 == userId || conversation.UserId2 == userId;

                if (isParticipated is false)
                {
                    return ApiResult<GetByIdChatProfile>.FailureResult($"User don't participated in chat[{chatId}]", ResultStatus.Unauthorized);
                }

                participantsCount = 2;

                if (conversation.User1.LastSeen.GetOnlineStatus() is true)
                {
                    participantsOnline++;
                }

                if (conversation.User2.LastSeen.GetOnlineStatus() is true)
                {
                    participantsOnline++;
                }

                avatarUrl = conversation.UserId1 == userId ? conversation.User2.AvatarUrl : conversation.User1.AvatarUrl;
                name = conversation.UserId1 == userId ? conversation.User2.Username : conversation.User1.Username;
                description = conversation.UserId1 == userId ? conversation.User2.Description : conversation.User1.Description;
                participants.Add(UserResponseDto.ConvertFromDomainObject(conversation.User1));
                participants.Add(UserResponseDto.ConvertFromDomainObject(conversation.User2));
            }
            else if (chat.Type == ChatType.Group)
            {
                var group = await _groupRepo.GetByIdAsync(chatId);

                bool isParticipated = group.Members.FirstOrDefault(m => m.UserId == userId) != null;

                if (isParticipated is not true)
                {
                    return ApiResult<GetByIdChatProfile>.FailureResult($"User don't participated in chat[{chatId}]", ResultStatus.Unauthorized);
                }

                participantsCount = group.Members.Count();

                foreach (var groupMember in group.Members)
                {
                    if (groupMember.User.LastSeen.GetOnlineStatus() is true)
                    {
                        participantsOnline++;
                    }
                    participants.Add(UserResponseDto.ConvertFromDomainObject(groupMember.User));
                }

                avatarUrl = group.AvatarUrl;
                name = group.Name;
                description = group.Description;
            }
            else if (chat.Type == ChatType.Channel)
            {
                var channel = await _channelRepo.GetByIdAsync(chatId);

                bool isParticipated = channel.Subscribers.FirstOrDefault(m => m.UserId == userId) != null;

                if (isParticipated is not true)
                {
                    return ApiResult<GetByIdChatProfile>.FailureResult($"User don't participated in chat[{chatId}]", ResultStatus.Unauthorized);
                }

                participantsCount = channel.Subscribers.Count();

                foreach (var channelSubscriber in channel.Subscribers)
                {
                    if (channelSubscriber.User.LastSeen.GetOnlineStatus() is true)
                    {
                        participantsOnline++;
                    }
                    participants.Add(UserResponseDto.ConvertFromDomainObject(channelSubscriber.User));
                }

                avatarUrl = channel.AvatarUrl;
                name = channel.Name;
                description = channel.Description;
            }

            var notificationsCount = await _notificationRepo.GetUserChatNotificationsCountAsync(userId, chatId);

            var model = new GetByIdChatProfile
            {
                ChatId = chatId,
                Description = description,
                AvatarUrl = avatarUrl,
                Name = name,
                ParticipantsCount = participantsCount,
                ParticipantsOnline = participantsOnline,
                Participants = participants
            };

            return ApiResult<GetByIdChatProfile>.SuccessResult(model);
        }

        public async Task<ApiResult<List<UserChatResponseDto>>> GetUserChatsAsync(Guid userId)
        {
            var conversationsApiResult = await _conversationService.GetUserConversationsAsync(userId);
            var groupsApiResult = await _groupService.GetUserParticipatedAsync(userId);
            var channelResult = await _channelService.GetUserSubscribedAsync(userId);

            var merged = new List<UserChatResponseDto>();

            merged.AddRange(conversationsApiResult.Data);
            merged.AddRange(groupsApiResult.Data);
            merged.AddRange(channelResult.Data);

            merged.OrderByDescending(m => (DateTimeOffset?)m.LastMessage.SentAt ?? DateTimeOffset.MinValue);

            return ApiResult<List<UserChatResponseDto>>.SuccessResult(merged);
        }

        public async Task<ApiResult<List<SearchChatResponseDto>>> SearchAsync(string term, Guid userId)
        {
            var usersApiResult = await _userService.SearchAsync(term, userId);
            var groupsApiResult = await _groupService.SearchAsync(term);
            var channelsApiResult = await _channelService.SearchAsync(term);

            var merged = new List<SearchChatResponseDto>();

            merged.AddRange(usersApiResult.Data);
            merged.AddRange(groupsApiResult.Data);
            merged.AddRange(channelsApiResult.Data);

            merged.OrderBy(m => m.EntityId);

            return ApiResult<List<SearchChatResponseDto>>.SuccessResult(merged);
        }

        public  async Task<ApiResult> UpdatePrivacyTypeAsync(Guid chatId, ChatPrivacyType chatPrivacyType)
        {
            var chat = await _repo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Chat with ID[{chatId}] not found");
            }

            chat.PrivacyType = chatPrivacyType;

            await _repo.UpdateAsync(chat);

            return ApiResult.SuccessResult();
        }
    }
}
