using Simpchat.Application.Errors;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;

using Simpchat.Application.Models.Reactions;
using Simpchat.Domain.Entities;
using Simpchat.Shared.Models;
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

        public async Task<Result<Guid>> CreateAsync(Guid messageId, Guid reactionId, Guid userId)
        {
            var reaction = await _repo.GetByIdAsync(reactionId);

            if (reaction is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.Reaction.IdNotFound);
            }

            var message = await _repo.GetByIdAsync(messageId);

            if (message is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.Message.IdNotFound);
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.User.IdNotFound);
            }

            var messageReaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = userId,
                ReactionId = reactionId
            };

            await _repo.CreateAsync(messageReaction);

            return messageReaction.Id;
        }

        public async Task<Result> DeleteAsync(Guid messageId, Guid userId)
        {
            var message = await _repo.GetByIdAsync(messageId);

            if (message is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.Message.IdNotFound);
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.User.IdNotFound);
            }

            var userReactionId = await _repo.GetIdAsync(messageId, userId);

            if (userReactionId is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.UserReaction.NotFoundWithUserIdAndReactionId);
            }

            var userReaction = await _repo.GetByIdAsync((Guid)userReactionId);

            if (userReaction is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.Reaction.IdNotFound);
            }

            await _repo.DeleteAsync(userReaction);

            return Result.Success();
        }
    }
}
