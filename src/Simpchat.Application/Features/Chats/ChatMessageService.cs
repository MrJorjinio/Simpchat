using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Post.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.Chats
{
    internal class ChatMessageService : IChatMessageService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public ChatMessageService(IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task<ApiResult> SendMessageAsync(PostMessageDto message, Guid currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return ApiResult.FailureResult($"User with ID[{currentUserId}] not found", ResultStatus.NotFound);
            }

            var addedMessage = await _messageRepository.AddMessageAsync(message, currentUser);

            if (addedMessage is null)
            {
                return ApiResult.FailureResult("Failed to add message", ResultStatus.Failure);
            }
            return ApiResult.SuccessResult();
        }
    }
}
