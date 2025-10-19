using Simpchat.Application.Common.Interfaces.External.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Chats.Post;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Channels;
using Simpchat.Domain.Entities.Groups;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.Channels
{
    public class ChannelService : IChannelService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IChatRepository _chatRepository;
        private readonly IChannelRepository _channelRepository;
        private const string BucketName = "channels-avatars";

        public ChannelService(IUserRepository userRepository, IChannelRepository channelRepository, IFileStorageService fileStorageService, IChatRepository chatRepository)
        {
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
            _chatRepository = chatRepository;
            _channelRepository = channelRepository;
        }

        public async Task<ApiResult> AddUserAsync(Guid chatId, Guid addingUserId, Guid currentUserId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat is null)
                return ApiResult.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);

            var addingUser = await _userRepository.GetByIdAsync(addingUserId);

            if (addingUser is null)
                return ApiResult.FailureResult($"Adding User with ID[{addingUserId} not found", ResultStatus.NotFound);

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return ApiResult.FailureResult($"User with ID[{currentUserId} not found", ResultStatus.NotFound);
            }

            await _channelRepository.AddSubscriberAsync(chat, addingUser, currentUser);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> AddUserPermissionAsync(string permissionName, Guid chatId, Guid addingUserId, Guid currentUserId)
        {
            var permission = await _chatRepository.GetPermissionByNameAsync(permissionName);

            if (permission is null)
            {
                return ApiResult.FailureResult($"Chat-Permission with NAME[{permissionName}] not found", ResultStatus.NotFound);
            }

            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat is null)
                return ApiResult.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);

            var addingUser = await _userRepository.GetByIdAsync(addingUserId);

            if (addingUser is null)
                return ApiResult.FailureResult($"Adding User with ID[{addingUserId} not found", ResultStatus.NotFound);

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return ApiResult.FailureResult($"User with ID[{currentUserId} not found", ResultStatus.NotFound);
            }

            await _channelRepository.AddUserPermissionAsync(permission, chat, addingUser, currentUser);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> CreateAsync(Guid userId, ChatPostDto chatPostDto, FileUploadRequest? avatar)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(chatPostDto?.Name))
                return ApiResult.FailureResult("Group name is required", ResultStatus.Failure);

            var chat = new Chat
            {
                Type = ChatType.Channel,
                PrivacyType = ChatPrivacyType.Public
            };

            chat = await _chatRepository.CreateAsync(chat);

            var channel = new Channel
            {
                Id = chat.Id,
                CreatedById = user.Id,
                Description = chatPostDto.Description,
                Name = chatPostDto.Name,
                Subscribers = new List<ChannelSubscriber>
                {
                    new ChannelSubscriber{ UserId = user.Id }
                }
            };

            channel.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);

            await _channelRepository.CreateAsync(channel);

            return ApiResult.SuccessResult();
        }
    }
}
