using Simpchat.Application.Errors;
using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;

using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Users;
using Simpchat.Domain.Enums;
using Simpchat.Shared.Models;

namespace Simpchat.Application.Features
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConversationRepository _conversationRepo;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "users-avatars";

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<Result<GetByIdUserDto>> GetByIdAsync(Guid userId, Guid currentUserId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure<GetByIdUserDto>(ApplicationErrors.User.IdNotFound);
            }

            var currentUser = await _userRepo.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return Result.Failure<GetByIdUserDto>(ApplicationErrors.User.IdNotFound);
            }

            var conversationBetweenId = await _conversationRepo.GetConversationBetweenAsync(userId, currentUserId);

            var model = new GetByIdUserDto
            {
                IsOnline = user.LastSeen.GetOnlineStatus(),
                Description = user.Description,
                AvatarUrl = user.AvatarUrl,
                ChatId = conversationBetweenId,
                LastSeen = user.LastSeen,
                UserId = userId,
                Username = user.Username
            };

            return model;
        }

        public async Task<Result<List<SearchChatResponseDto>>> SearchAsync(string term, Guid userId)
        {
            var users = await _userRepo.SearchAsync(term);

            var modeledUsers = new List<SearchChatResponseDto>();

            foreach (var user in users)
            {
                var conversationBetweenId = await _conversationRepo.GetConversationBetweenAsync(userId, user.Id);

                var model = new SearchChatResponseDto
                {
                    EntityId = user.Id,
                    DisplayName = user.Description,
                    AvatarUrl = user.AvatarUrl,
                    ChatType = ChatTypes.Conversation,
                    ChatId = conversationBetweenId
                };

                modeledUsers.Add(model);
            }

            return modeledUsers;
        }

        public async Task<Result> SetLastSeenAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure(ApplicationErrors.User.IdNotFound);
            }

            user.LastSeen = DateTimeOffset.UtcNow;

            await _userRepo.UpdateAsync(user);

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(Guid userId, UpdateUserDto updateUserDto, UploadFileRequest? avatar)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure(ApplicationErrors.User.IdNotFound);
            }

            if (_userRepo.SearchAsync(user.Username) is not null)
            {
                return Result.Failure(ApplicationErrors.User.UsernameNotFound);
            }

            if (avatar is not null)
            {
                if (avatar.FileName != null && avatar.Content != null && avatar.ContentType != null)
                {
                    user.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);
                }
            }

            user.Username = updateUserDto.Username;
            user.Description = updateUserDto.Description;
            user.HwoCanAddType = updateUserDto.AddChatMinLvl;

            await _userRepo.UpdateAsync(user);

            return Result.Success();
        }
    }
}
