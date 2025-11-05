using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Reactions;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features
{
    internal class MessageReactionService : IMessageReactionService
    {
        private readonly IMessageReactionRepository _repo;
        private readonly IReactionRepository _reactionRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMessageRepository _messageRepo;
        public MessageReactionService(
            IMessageReactionRepository repo,
            IReactionRepository reactionRepo,
            IChatRepository chatRepo,
            IUserRepository userRepo,
            IMessageRepository messageRepo)
        {
            _repo = repo;
            _reactionRepo = reactionRepo;
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
        }

        public async Task<ApiResult<Guid>> CreateAsync(Guid messageId, Guid reactionId, Guid userId)
        {
            var reaction = await _repo.GetByIdAsync(reactionId);

            if (reaction is null)
            {
                return ApiResult<Guid>.FailureResult($"Reaction with ID[{reactionId} not found");
            }

            var message = await _repo.GetByIdAsync(messageId);

            if (message is null)
            {
                return ApiResult<Guid>.FailureResult($"Message with ID[{messageId}] not found");
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<Guid>.FailureResult($"User with ID[{userId} not found");
            }

            var messageReaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = userId,
                ReactionId = reactionId
            };

            await _repo.CreateAsync(messageReaction);

            return ApiResult<Guid>.SuccessResult(messageReaction.Id);
        }

        public async Task<ApiResult> DeleteAsync(Guid messageId, Guid userId)
        {
            var message = await _repo.GetByIdAsync(messageId);

            if (message is null)
            {
                return ApiResult.FailureResult($"Message with ID[{messageId}] not found");
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId} not found");
            }

            var userReactionId = await _repo.GetIdAsync(messageId, userId);

            if (userReactionId is null)
            {
                return ApiResult.FailureResult($"UserReaction with MESSAGE_ID[{messageId}] and USER_ID[{userId}] not found");
            }

            var userReaction = await _repo.GetByIdAsync((Guid)userReactionId);

            if (userReaction is null)
            {
                return ApiResult.FailureResult($"Reaction with ID[{userReactionId}");
            }

            await _repo.DeleteAsync(userReaction);

            return ApiResult.SuccessResult();
        }
    }
}
