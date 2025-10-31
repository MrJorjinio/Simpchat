using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories.New;
using Simpchat.Application.Interfaces.Repositories.Old;
using Simpchat.Application.Interfaces.Services.New;
using Simpchat.Application.Interfaces.Services.Old;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Channels;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.New
{
    public class NewChannelService : INewChannelService
    {
        private readonly INewChannelRepository _repo;
        private readonly INewUserRepository _userRepo;
        private readonly INewChatRepository _chatRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly INewNotificationRepository _notificationRepo;
        private readonly INewMessageRepository _messageRepo;
        private const string BucketName = "channels-avatars";

        public NewChannelService(
            INewChannelRepository repo,
            INewUserRepository userRepo,
            INewChatRepository chatRepo,
            IFileStorageService fileStorageService,
            INewNotificationRepository notificationRepository,
            INewMessageRepository messageRepository)
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

        public async Task<ApiResult> CreateAsync(PostChatDto chatPostDto)
        {
            var user = await _userRepo.GetByIdAsync(chatPostDto.OwnerId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{chatPostDto.OwnerId}] not found", ResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(chatPostDto?.Name))
                return ApiResult.FailureResult("Channel name is required", ResultStatus.Failure);

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

            if (chatPostDto.Avatar is not null)
            {
                if (chatPostDto.Avatar.FileName != null && chatPostDto.Avatar.Content != null && chatPostDto.Avatar.ContentType != null)
                {
                    channel.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, chatPostDto.Avatar.FileName, chatPostDto.Avatar.Content, chatPostDto.Avatar.ContentType);
                }
            }

            await _repo.CreateAsync(channel);

            return ApiResult.SuccessResult();
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

        public async Task<ApiResult> UpdateAsync(Guid channelId, PostChatDto updateChatDto)
        {
            var channel = await _repo.GetByIdAsync(channelId);

            if (channel is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{channelId}] not found");
            }

            channel.Name = updateChatDto.Name;
            channel.Description = updateChatDto.Description;

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

            return ApiResult<List<UserChatResponseDto>>.SuccessResult(modeledChannels);
        }
    }
}
