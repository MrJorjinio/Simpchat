using Simpchat.Application.Common.Interfaces.External.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Chats.Get.ById;
using Simpchat.Application.Common.Models.Chats.Get.Profile;
using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Application.Common.Models.Chats.Post.Message;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Application.Common.Models.Pagination;
using Simpchat.Application.Common.Models.Pagination.Chat;
using Simpchat.Domain.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.Chats
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "chat-avatars";

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository, IFileStorageService fileStorageService)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResult<ICollection<UserChatResponseDto>?>> GetUserChatsAsync(Guid userId)
        {
            if (_userRepository.GetByIdAsync(userId) is null)
            {
                return ApiResult<ICollection<UserChatResponseDto>>.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }
            return ApiResult<ICollection<UserChatResponseDto>>.SuccessResult(await _chatRepository.GetUserChatsAsync(userId));
        }

        public async Task<ApiResult<PaginationResult<ChatSearchResponseDto>>> SearchByNameAsync(ChatSearchPageModel chatSearchDto, Guid currentUserId)
        {
            if (await _userRepository.GetByIdAsync(currentUserId) is null)
            {
                ApiResult<ICollection<ChatSearchResponseDto>>.FailureResult($"User with ID[{currentUserId}] not found", ResultStatus.NotFound);
            }

            var chats = await _chatRepository.SearchByNameAsync(chatSearchDto.searchTerm, currentUserId);

            var filteredChats = chats
                .Skip(chatSearchDto.PageSize * (chatSearchDto.PageNumber - 1))
                .Take(chatSearchDto.PageSize)
                .Select(c => new ChatSearchResponseDto
                {
                    ChatId = c.ChatId,
                    DisplayName = c.DisplayName,
                    AvatarUrl = c.AvatarUrl,
                    EntityId = c.EntityId,
                    ChatType = c.ChatType
                }).ToList();

            var paginationResult = new PaginationResult<ChatSearchResponseDto?>
            {
                Data = filteredChats,
                PageNumber = chatSearchDto.PageNumber,
                PageSize = chatSearchDto.PageSize,
                TotalCount = chats.Count
            };

            return ApiResult<PaginationResult<ChatSearchResponseDto>>.SuccessResult(paginationResult);
        }

        public async Task<ApiResult<ChatGetByIdDto>> GetByIdAsync(Guid chatId, Guid userId)
        {
            if (_userRepository.GetByIdAsync(userId) is null)
            {
                return ApiResult<ChatGetByIdDto>.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            var chat = await _chatRepository.GetByIdAsync(chatId, userId);

            if (chat is null)
            {
                return ApiResult<ChatGetByIdDto>.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);
            }

            return ApiResult<ChatGetByIdDto>.SuccessResult(chat);
        }

        public async Task<ApiResult<ChatGetByIdProfile>> GetProfileByIdAsync(Guid chatId, Guid userId)
        {
            if (await _userRepository.GetByIdAsync(userId) is null)
            {
                return ApiResult<ChatGetByIdProfile>.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            var chat = await _chatRepository.GetProfileByIdAsync(chatId, userId);

            if (chat is null)
            {
                return ApiResult<ChatGetByIdProfile>.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);
            }

            return ApiResult<ChatGetByIdProfile>.SuccessResult(chat);
        }

        public async Task<ApiResult> SendMessageAsync(MessagePostDto message, Guid currentUserId)
        {
            await _chatRepository.AddMessageAsync(message, currentUserId);
            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> UpdateAvatarAsync(Guid chatId, Guid userId, FileUploadRequest file)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat is null)
            {
                return ApiResult.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);
            }

            if (chat.Type == ChatType.Conversation)
            {
                return ApiResult.FailureResult($"You can't update avatar for opposite conversation user", ResultStatus.NotFound);
            }
            else if (chat.Type == ChatType.Group)
            {
                chat.Group.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, file.FileName, file.Content, file.ContentType);
            }
            else
            {
                chat.Channel.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, file.FileName, file.Content, file.ContentType);
            }

            await _chatRepository.UpdateAsync(chat);
            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> UpdatePrivacyTypeAsync(Guid chatId, Guid userId, ChatPrivacyType privacyType)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat is null)
            {
                return ApiResult.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);
            }

            chat.PrivacyType = privacyType;
            await _chatRepository.UpdateAsync(chat);

            return ApiResult.SuccessResult();
        }
    }
}
