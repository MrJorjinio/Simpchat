using Simpchat.Application.Common.Pagination;
using Simpchat.Application.Common.Pagination.Chat;
using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Get.ById;
using Simpchat.Application.Models.Chats.Get.Profile;
using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
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

        public async Task<ApiResult<PaginationResult<SearchChatResponseDto>>> SearchByNameAsync(ChatSearchPageModel chatSearchDto, Guid currentUserId)
        {
            if (await _userRepository.GetByIdAsync(currentUserId) is null)
            {
                return ApiResult<PaginationResult<SearchChatResponseDto>>.FailureResult($"User with ID[{currentUserId}] not found", ResultStatus.NotFound);
            }

            var chats = await _chatRepository.SearchByNameAsync(chatSearchDto.searchTerm, currentUserId);

            var filteredChats = chats
                .Skip(chatSearchDto.PageSize * (chatSearchDto.PageNumber - 1))
                .Take(chatSearchDto.PageSize)
                .Select(c => new SearchChatResponseDto
                {
                    ChatId = c.ChatId,
                    DisplayName = c.DisplayName,
                    AvatarUrl = c.AvatarUrl,
                    EntityId = c.EntityId,
                    ChatType = c.ChatType
                }).ToList();

            var paginationResult = new PaginationResult<SearchChatResponseDto?>
            {
                Data = filteredChats,
                PageNumber = chatSearchDto.PageNumber,
                PageSize = chatSearchDto.PageSize,
                TotalCount = chats.Count
            };

            return ApiResult<PaginationResult<SearchChatResponseDto>>.SuccessResult(paginationResult);
        }

        public async Task<ApiResult<GetByIdChatDto>> GetByIdAsync(Guid chatId, Guid userId)
        {
            if (_userRepository.GetByIdAsync(userId) is null)
            {
                return ApiResult<GetByIdChatDto>.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            var chat = await _chatRepository.GetByIdAsync(chatId, userId);

            if (chat is null)
            {
                return ApiResult<GetByIdChatDto>.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);
            }

            return ApiResult<GetByIdChatDto>.SuccessResult(chat);
        }

        public async Task<ApiResult<GetByIdChatProfile>> GetProfileByIdAsync(Guid chatId, Guid userId)
        {
            if (await _userRepository.GetByIdAsync(userId) is null)
            {
                return ApiResult<GetByIdChatProfile>.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            var chat = await _chatRepository.GetProfileByIdAsync(chatId, userId);

            if (chat is null)
            {
                return ApiResult<GetByIdChatProfile>.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);
            }

            return ApiResult<GetByIdChatProfile>.SuccessResult(chat);
        }

        public async Task<ApiResult> UpdateAvatarAsync(Guid chatId, Guid userId, UploadFileRequest file)
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
