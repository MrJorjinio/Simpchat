using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<ApiResult<Guid>> BanUserAsync(Guid chatId, Guid userId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult<Guid>.FailureResult($"Chat with ID[{chatId}] not found");
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<Guid>.FailureResult($"User with ID{userId} not found");
            }

            var chatBan = new ChatBan
            {
                ChatId = chatId,
                UserId = userId
            };

            await _repo.CreateAsync(chatBan);

            return ApiResult<Guid>.SuccessResult(chatBan.Id);
        }

        public async Task<ApiResult> DeleteAsync(Guid chatId, Guid userId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Chat with ID[{chatId}] not found");
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID{userId} not found");
            }

            var chatBan = new ChatBan
            {
                ChatId = chatId,
                UserId = userId
            };

            await _repo.DeleteAsync(chatBan);

            return ApiResult.SuccessResult();
        }
    }
}
