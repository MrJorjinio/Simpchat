using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories.Old;
using Simpchat.Application.Interfaces.Services.Old;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Groups;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = SimpchatWeb.Services.Db.Contexts.Default.Entities.Group;

namespace Simpchat.Application.Features.Old.Groups
{
    internal class GroupService : IGroupService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IChatRepository _chatRepository;
        private const string BucketName = "groups-avatars";

        public GroupService(IUserRepository userRepository, IGroupRepository groupRepository, IFileStorageService fileStorageService, IChatRepository chatRepository)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _fileStorageService = fileStorageService;
            _chatRepository = chatRepository;
        }

        public async Task<ApiResult> CreateAsync(Guid userId, PostChatDto chatPostDto, UploadFileRequest? avatar)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found", ResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(chatPostDto?.Name))
                return ApiResult.FailureResult("Group name is required", ResultStatus.Failure);

            var chat = new Chat
            {
                Type = ChatType.Group,
                PrivacyType = ChatPrivacyType.Public
            };

            chat = await _chatRepository.CreateAsync(chat);

            var group = new Group
            {
                Id = chat.Id,
                CreatedById = user.Id,
                Description = chatPostDto.Description,
                Name = chatPostDto.Name,
                Members = new List<GroupMember>
                {
                    new GroupMember{ UserId = user.Id }
                }
            };

            if (avatar is not null)
            {
                if (avatar.FileName != null && avatar.Content != null && avatar.ContentType != null)
                {
                    group.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);
                }
            }

            await _groupRepository.CreateAsync(group);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> AddUserAsync(Guid chatId, Guid addingUserId, Guid currentUserId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat is null)
                return ApiResult.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);

            var addingUser = await _userRepository.GetByIdAsync(addingUserId);

            if (addingUser is null)
                return ApiResult.FailureResult($"Adding User with ID[{addingUserId} not found", ResultStatus.NotFound);

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return ApiResult.FailureResult($"User with ID[{currentUserId} not found", ResultStatus.NotFound);
            }

            await _groupRepository.AddMemberAsync(chat, addingUser, currentUser);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> AddUserPermissionAsync(string permissionName, Guid chatId, Guid addingUserId, Guid currentUserId)
        {
            var permission = await _chatRepository.GetPermissionByNameAsync(permissionName);

            if (permission is null)
            {
                return ApiResult.FailureResult($"Chat-Permission with NAME[{permissionName}] not found", ResultStatus.NotFound);
            }

            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat is null)
                return ApiResult.FailureResult($"Chat with ID[{chatId}] not found", ResultStatus.NotFound);

            var addingUser = await _userRepository.GetByIdAsync(addingUserId);

            if (addingUser is null)
                return ApiResult.FailureResult($"Adding User with ID[{addingUserId} not found", ResultStatus.NotFound);

            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser is null)
            {
                return ApiResult.FailureResult($"User with ID[{currentUserId} not found", ResultStatus.NotFound);
            }

            await _groupRepository.AddUserPermissionAsync(permission, chat, addingUser, currentUser);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteMemberAsync(Guid userId, Guid channelId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found");
            }

            var chat = await _chatRepository.GetByIdAsync(channelId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Group with ID[{channelId}] not found");
            }

            if (chat.Type != ChatType.Group)
            {
                return ApiResult.FailureResult($"Can't delete from TYPE[{chat.Type}]");
            }

            await _groupRepository.DeleteMemberAsync(user, chat.Group);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{chatId}] not found");
            }

            if (chat.Type != ChatType.Group)
            {
                return ApiResult.FailureResult($"Can't delete from TYPE[{chat.Type}]");
            }

            await _groupRepository.DeleteAsync(chat.Group);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> UpdateAsync(Guid chatId, PostChatDto updateChatDto)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Channel with ID[{chatId}] not found");
            }

            chat.Group.Name = updateChatDto.Name;
            chat.Group.Description = updateChatDto.Description;

            await _chatRepository.UpdateAsync(chat);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<ICollection<SearchChatResponseDto>?>> SearchAsync(string searchTerm)
        {
            var response = await _groupRepository.SearchByNameAsync(searchTerm);

            return ApiResult<ICollection<SearchChatResponseDto>>.SuccessResult(response);
        }
    }
}
