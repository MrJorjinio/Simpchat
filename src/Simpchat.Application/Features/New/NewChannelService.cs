using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories.New;
using Simpchat.Application.Interfaces.Repositories.Old;
using Simpchat.Application.Interfaces.Services.Old;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
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
    public class NewChannelService
    {
        private readonly INewChannelRepository _repo;
        private readonly INewUserRepository _userRepo;
        private readonly INewChatRepository _chatRepo;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "channels-avatars";

        public NewChannelService(
            INewChannelRepository repo,
            INewUserRepository userRepo, 
            INewChatRepository chatRepo,
            IFileStorageService fileStorageService
            )
        {
            _repo = repo;
            _userRepo = userRepo;
            _chatRepo = chatRepo;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResult> AddUserAsync(Guid chatId, Guid addingUserId)
        {
            await _repo.AddSubscriberAsync(addingUserId, chatId);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> CreateAsync(Guid userId, PostChatDto chatPostDto, UploadFileRequest? avatar)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
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

            if (avatar is not null)
            {
                if (avatar.FileName != null && avatar.Content != null && avatar.ContentType != null)
                {
                    channel.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);
                }
            }

            await _repo.CreateAsync(channel);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteAsync(Guid chatId)
        {
            var channel = await _repo.GetByIdAsync(chatId);

            if (channel is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{chatId}] not found");
            }

            await _repo.DeleteAsync(channel);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteSubscriberAsync(Guid userId, Guid channelId)
        {
            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<List<SearchChatResponseDto>?>> SearchAsync(string searchTerm)
        {
            var results = await _repo.SearchAsync(searchTerm);
            return await ApiResult<List<SearchChatResponseDto>>.SuccessResult(results);
        }

        public Task<ApiResult> UpdateAsync(Guid chatId, PostChatDto updateChatDto)
        {
            throw new NotImplementedException();
        }
    }
}
