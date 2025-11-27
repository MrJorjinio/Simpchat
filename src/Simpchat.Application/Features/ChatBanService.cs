using Simpchat.Application.Errors;
using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;
using Simpchat.Shared.Models;

namespace Simpchat.Application.Features
{
    public class ChatBanService : IChatBanService
    {
        private readonly IChatBanRepository _repo;
        private readonly IChatRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IChannelRepository _channelRepo;
        private readonly IChatUserPermissionRepository _chatUserPermissionRepo;

        public ChatBanService(
            IChatBanRepository repo,
            IChatRepository chatRepo,
            IUserRepository userRepo,
            IGroupRepository groupRepo,
            IChannelRepository channelRepo,
            IChatUserPermissionRepository chatUserPermissionRepo)
        {
            _repo = repo;
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _channelRepo = channelRepo;
            _chatUserPermissionRepo = chatUserPermissionRepo;
        }

        public async Task<Result<Guid>> BanUserAsync(Guid chatId, Guid userId, Guid requesterId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.Chat.IdNotFound);
            }

            if (!await CanBanUserAsync(chatId, requesterId, chat.Type))
            {
                return Result.Failure<Guid>(ApplicationErrors.ChatPermission.Denied);
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

        public async Task<Result> DeleteAsync(Guid chatId, Guid userId, Guid requesterId)
        {
            var chat = await _chatRepo.GetByIdAsync(chatId);

            if (chat is null)
            {
                return Result.Failure(ApplicationErrors.Chat.IdNotFound);
            }

            if (!await CanBanUserAsync(chatId, requesterId, chat.Type))
            {
                return Result.Failure(ApplicationErrors.ChatPermission.Denied);
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure(ApplicationErrors.User.IdNotFound);
            }

            var chatBan = new ChatBan
            {
                ChatId = chatId,
                UserId = userId
            };

            await _repo.DeleteAsync(chatBan);

            return Result.Success();
        }

        private async Task<bool> CanBanUserAsync(Guid chatId, Guid requesterId, ChatTypes chatType)
        {
            if (chatType == ChatTypes.Group)
            {
                var group = await _groupRepo.GetByIdAsync(chatId);
                if (group == null) return false;

                return group.IsGroupOwner(requesterId) ||
                       await _chatUserPermissionRepo.HasUserPermissionAsync(chatId, requesterId, nameof(ChatPermissionTypes.ManageUsers));
            }
            else if (chatType == ChatTypes.Channel)
            {
                var channel = await _channelRepo.GetByIdAsync(chatId);
                if (channel == null) return false;

                return channel.IsChannelOwner(requesterId) ||
                       await _chatUserPermissionRepo.HasUserPermissionAsync(chatId, requesterId, nameof(ChatPermissionTypes.ManageUsers));
            }

            return false;
        }
    }
}
