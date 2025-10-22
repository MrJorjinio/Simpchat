using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.Conversations
{
    public class ConversationService : IConversationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(IUserRepository userRepository, IConversationRepository conversationRepository)
        {
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
        }

        public async Task<ApiResult> DeleteAsync(Guid userId1, Guid userId2)
        {
            var user1 = await _userRepository.GetByIdAsync(userId1);

            if (user1 is null)
            {
                return ApiResult.FailureResult($"User1 with ID[{userId1}] not found");
            }

            var user2 = await _userRepository.GetByIdAsync(userId2);

            if (user2 is null)
            {
                return ApiResult.FailureResult($"User1 with ID[{userId2}] not found");
            }

            await _conversationRepository.DeleteAsync(user1, user2);

            return ApiResult.SuccessResult();
        }
    }
}
