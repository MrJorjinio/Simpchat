using Simpchat.Application.Common.Interfaces.External.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Chats.Messages;
using Simpchat.Application.Common.Models.Chats.Response;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.Chats
{
    internal class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "messages-files";

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository, IFileStorageService fileStorageService)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
        }
        public async Task<ApiResult<ChatGetByIdResponseDto>> GetByIdAsync(Guid chatId, Guid userId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            
            if (chat is null)
            {
                return ApiResult<ChatGetByIdResponseDto>.FailureResult($"Chat with ID[{chatId}] not found", );
            }

            var data = ChatGetByIdResponseDto.ConvertFromDomainObject(chat, userId);
            return ApiResult<ChatGetByIdResponseDto>.SuccessResult(data);
        }

        public async Task<ApiResult<ICollection<UserChatResponseDto>?>> GetUserChatsAsync(Guid userId)
        {
            if (await _userRepository.GetByIdAsync(userId) is null)
            {
                return ApiResult<ICollection<UserChatResponseDto>?>.FailureResult($"User with ID[{userId} not found", ResultStatus.NotFound);
            }

            var chats = await _chatRepository.GetUserChatsAsync(userId);

            var data = chats
                .Select(c => UserChatResponseDto.ConvertFromDomainObject(c, userId))
                .ToList();

            return ApiResult<ICollection<UserChatResponseDto>?>.SuccessResult(data);
        }

        public async Task<ApiResult<ICollection<ChatGetByIdResponseDto>>?> SearchByNameAsync(string searchTerm, Guid userId)
        {
            if (await _userRepository.GetByIdAsync(userId) is null)
            {
                return ApiResult<ICollection<ChatGetByIdResponseDto>>.FailureResult($"User with ID[{userId}] not found");
            }

            var chats = await _chatRepository.SearchByNameAsync(searchTerm, userId);

            var data = chats
                .Select(c => ChatGetByIdResponseDto.ConvertFromDomainObject(c, userId))
                .ToList();

            return ApiResult<ICollection<ChatGetByIdResponseDto>>.SuccessResult(data);
        }

        public async Task<ApiResult> SendMessageAsync(ChatSendMessageDto model)
        {
            if (await _userRepository.GetByIdAsync(model.SenderId) is null)
            {
                return ApiResult.FailureResult($"User with ID[{model.SenderId}] not found");
            }

            var fileUrl = "";
            if (model.FileType != null && model.FileName != null && model.FileStream != null)
            {
                fileUrl = await _fileStorageService.UploadFileAsync(BucketName, model.FileName, model.FileStream, model.FileType);
            }

            var message = ChatSendMessageDto.ToDomainObject(model);
            message.FileUrl = fileUrl;

            await _chatRepository.AddMessageAsync(message);

            return ApiResult.SuccessResult();
        }
    }
}
