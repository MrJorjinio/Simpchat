using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Channels;
using Simpchat.Domain.Entities.Groups;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Channel = SimpchatWeb.Services.Db.Contexts.Default.Entities.Channel;

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

        public async Task<ApiResult> CreateAsync(Guid userId, PostChatDto chatPostDto, UploadFileRequest? avatar)
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

            if (avatar is not null)
            {
                if (avatar.FileName != null && avatar.Content != null && avatar.ContentType != null)
                {
                    channel.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);
                }
            }

            await _channelRepository.CreateAsync(channel);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteSubscriberAsync(Guid userId, Guid channelId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found");
            }

            var chat = await _chatRepository.GetByIdAsync(channelId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{channelId}] not found");
            }

            if (chat.Type != ChatType.Channel)
            {
                return ApiResult.FailureResult($"Can't delete from Channel TYPE[{chat.Type}]");
            }

            await _channelRepository.DeleteSubscriberAsync(user, chat.Channel);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{chatId}] not found");
            }

            if (chat.Type != ChatType.Channel)
            {
                return ApiResult.FailureResult($"Can't delete from Channel TYPE[{chat.Type}]");
            }

            await _channelRepository.DeleteAsync(chat.Channel);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> UpdateAsync(Guid chatId, PostChatDto updateChatDto)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{chatId}] not found");
            }

            chat.Channel.Name = updateChatDto.Name;
            chat.Channel.Description = updateChatDto.Description;

            await _chatRepository.UpdateAsync(chat);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<ICollection<SearchChatResponseDto>?>> SearchAsync(string searchTerm)
        {
            var response = await _channelRepository.SearchByNameAsync(searchTerm);

            return ApiResult<ICollection<SearchChatResponseDto>>.SuccessResult(response);
        }
    }
}
