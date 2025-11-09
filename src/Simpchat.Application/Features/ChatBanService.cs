using Simpchat.Application.Errors;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Domain.Entities;
using Simpchat.Shared.Models;

namespace Simpchat.Application.Features
{
    public class ChatBanService : IChatBanService
    {
        private readonly IChatBanRepository _repo;
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;

        public ChatBanService(IChatBanRepository repo, IChatRepository chatRepo, IUserRepository userRepo)
        {
            _repo = repo;
            _chatRepo = chatRepo;
            _userRepo = userRepo;
        }

        public async Task<Result<Guid>> BanUserAsync(Guid chatId, Guid userId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.Chat.IdNotFound);
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.User.IdNotFound);
            }

            var chatBan = new ChatBan
            {
                ChatId = chatId,
                UserId = userId
            };

            await _repo.CreateAsync(chatBan);

            return chatBan.Id;
        }

        public async Task<Result> DeleteAsync(Guid chatId, Guid userId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return Result.Failure(ApplicationErrors.Chat.IdNotFound);
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure(ApplicationErrors.Chat.IdNotFound);
            }

            var chatBan = new ChatBan
            {
                ChatId = chatId,
                UserId = userId
            };

            await _repo.DeleteAsync(chatBan);

            return Result.Success();
        }
    }
}
