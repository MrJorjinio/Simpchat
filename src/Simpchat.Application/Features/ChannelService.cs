using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Messages;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;

namespace Simpchat.Application.Features
{
    public class ChannelService : IChannelService
    {
        private readonly IChannelRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationRepository _notificationRepo;
        private readonly IMessageRepository _messageRepo;
        private const string BucketName = "channels-avatars";

        public ChannelService(
            IChannelRepository repo,
            IUserRepository userRepo,
            IChatRepository chatRepo,
            IFileStorageService fileStorageService,
            INotificationRepository notificationRepository,
            IMessageRepository messageRepository)
        {
            _repo = repo;
            _userRepo = userRepo;
            _chatRepo = chatRepo;
            _fileStorageService = fileStorageService;
            _notificationRepo = notificationRepository;
            _messageRepo = messageRepository;
        }

        public async Task<ApiResult> AddSubscriberAsync(Guid channelId, Guid userId)
        {
            var chat = await _repo.GetByIdAsync(channelId);

            if (chat is null)
                return ApiResult.FailureResult($"Chat with ID[{channelId}] not found", ResultStatus.NotFound);

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
                return ApiResult.FailureResult($"Adding User with ID[{userId} not found", ResultStatus.NotFound);

            await _repo.AddSubscriberAsync(userId, channelId);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<Guid>> CreateAsync(PostChatDto chatPostDto, UploadFileRequest? avatar)
        {
            var user = await _userRepo.GetByIdAsync(chatPostDto.OwnerId);

            if (user is null)
            {
                return ApiResult<Guid>.FailureResult($"User with ID[{chatPostDto.OwnerId}] not found", ResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(chatPostDto?.Name))
                return ApiResult<Guid>.FailureResult("Channel name is required", ResultStatus.Failure);

            var chat = new Chat
            {
                Type = ChatType.Channel,
                PrivacyType = ChatPrivacyType.Public
            };

            var chatId = await _chatRepo.CreateAsync(chat);

            var channel = new Channel
            {
                Id = chatId,
                CreatedById = user.Id,
                Description = chatPostDto.Description,
                Name = chatPostDto.Name,
                Subscribers = new List<ChannelSubscriber>
                {
                    new ChannelSubscriber{ UserId = user.Id }
                }
            };

            if (avatar is not null)
            {
                if (avatar.FileName != null && avatar.Content != null && avatar.ContentType != null)
                {
                    channel.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);
                }
            }

            await _repo.CreateAsync(channel);

            return ApiResult<Guid>.SuccessResult(channel.Id);
        }

        public async Task<ApiResult> DeleteAsync(Guid channelId)
        {
            var channel = await _repo.GetByIdAsync(channelId);

            if (channel is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{channelId}] not found");
            }

            await _repo.DeleteAsync(channel);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteSubscriberAsync(Guid userId, Guid channelId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found");
            }

            var chat = await _chatRepo.GetByIdAsync(channelId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{channelId}] not found");
            }

            if (chat.Type != ChatType.Channel)
            {
                return ApiResult.FailureResult($"Can't delete from Chat TYPE[{chat.Type}]");
            }

            var channelSubscriber = new ChannelSubscriber
            {
                ChannelId = channelId,
                UserId = userId
            };

            await _repo.DeleteSubscriberAsync(channelSubscriber);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<List<SearchChatResponseDto>?>> SearchAsync(string searchTerm)
        {
            var results = await _repo.SearchAsync(searchTerm);

            var modeledResults = results
                .Select(r => new SearchChatResponseDto
                {
                    ChatId = r.Id,
                    AvatarUrl = r.AvatarUrl,
                    DisplayName = r.Name,
                    ChatType = ChatType.Channel
                })
                .ToList();

            return ApiResult<List<SearchChatResponseDto>>.SuccessResult(modeledResults);
        }

        public async Task<ApiResult> UpdateAsync(Guid channelId, UpdateChatDto updateChatDto, UploadFileRequest? avatar)
        {
            var channel = await _repo.GetByIdAsync(channelId);

            if (channel is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{channelId}] not found");
            }

            channel.Name = updateChatDto.Name;
            channel.Description = updateChatDto.Description;

            if (avatar is not null)
            {
                if (avatar.FileName != null && avatar.Content != null && avatar.ContentType != null)
                {
                    channel.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content,avatar.ContentType);
                }
            }

            await _repo.UpdateAsync(channel);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<List<UserChatResponseDto>>> GetUserSubscribedAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<List<UserChatResponseDto>>.FailureResult($"User with ID[{userId}] not found");
            }

            var channels = await _repo.GetUserSubscribedChannelsAsync(userId);

            var modeledChannels = new List<UserChatResponseDto>();

            foreach (var channel in channels)
            {
                var notificationsCount = await _notificationRepo.GetUserChatNotificationsCountAsync(userId, channel.Id);
                var lastMessage = await _messageRepo.GetLastMessageAsync(channel.Id);
                var lastUserSendedMessage = await _messageRepo.GetUserLastSendedMessageAsync(userId, channel.Id);

                var modeledChannel = new UserChatResponseDto
                {
                    Id = channel.Id,
                    AvatarUrl = channel.AvatarUrl,
                    LastMessage = new LastMessageResponseDto
                    {
                        Content = lastMessage.Content,
                        FileUrl = lastMessage.FileUrl,
                        SenderUsername = lastMessage.Sender.Username,
                        SentAt = lastMessage.SentAt
                    },
                    Name = channel.Name,
                    NotificationsCount = notificationsCount,
                    Type = ChatType.Channel,
                    UserLastMessage = lastUserSendedMessage?.SentAt
                };

                modeledChannels.Add(modeledChannel);
            }

            modeledChannels.OrderByDescending(mc => (DateTimeOffset?)mc.LastMessage.SentAt ?? DateTimeOffset.MinValue);

            return ApiResult<List<UserChatResponseDto>>.SuccessResult(modeledChannels);
        }
    }
}
