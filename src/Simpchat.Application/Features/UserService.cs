using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Get.ById;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Users.GetById;
using Simpchat.Application.Models.Users.Update;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features
{
    internal class UserService : IUserService
    {
        private readonly INewUserRepository _userRepo;
        private readonly IConversationRepository _conversationRepo;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "users-avatars";

        public UserService(INewUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ApiResult<GetByIdUserDto>> GetByIdAsync(Guid userId, Guid currentUserId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<GetByIdUserDto>.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            var currentUser = await _userRepo.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return ApiResult<GetByIdUserDto>.FailureResult($"User with ID[{currentUserId}] not found", ResultStatus.NotFound);
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

            return ApiResult<GetByIdUserDto>.SuccessResult(model);
        }

        public async Task<ApiResult<List<SearchChatResponseDto>>> SearchAsync(string term, Guid userId)
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
                    ChatType = ChatType.Conversation,
                    ChatId = conversationBetweenId
                };

                modeledUsers.Add(model);
            }

            return ApiResult<List<SearchChatResponseDto>>.SuccessResult(modeledUsers);
        }

        public async Task<ApiResult> SetLastSeenAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            user.LastSeen = DateTimeOffset.UtcNow;

            await _userRepo.UpdateAsync(user);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> UpdateAsync(Guid userId, UpdateUserDto updateUserDto, UploadFileRequest? avatar)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            if (_userRepo.SearchAsync(user.Username) is not null)
            {
                return ApiResult.FailureResult($"USERNAME[{updateUserDto.Username}] is already exists");
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
            user.ChatMemberAddPermissionType = updateUserDto.AddChatMinLvl;

            await _userRepo.UpdateAsync(user);

            return ApiResult.SuccessResult();
        }
    }
}
