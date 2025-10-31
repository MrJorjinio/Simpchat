using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories.New;
using Simpchat.Application.Interfaces.Services.New;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Groups;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.New
{
    public class NewGroupService : INewGroupService
    {
        private readonly INewGroupRepository _repo;
        private readonly INewUserRepository _userRepo;
        private readonly INewChatRepository _chatRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly INewNotificationRepository _notificationRepo;
        private readonly INewMessageRepository _messageRepo;
        private const string BucketName = "groups-avatars";

        public NewGroupService(
            INewGroupRepository repo,
            INewUserRepository userRepo,
            INewChatRepository chatRepo,
            IFileStorageService fileStorageService,
            INewNotificationRepository notificationRepository,
            INewMessageRepository messageRepository)
        {
            _repo = repo;
            _userRepo = userRepo;
            _chatRepo = chatRepo;
            _fileStorageService = fileStorageService;
            _notificationRepo = notificationRepository;
            _messageRepo = messageRepository;
        }

        public async Task<ApiResult> AddMemberAsync(Guid groupId, Guid userId)
        {
            var group = await _repo.GetByIdAsync(groupId);

            if (group is null)
                return ApiResult.FailureResult($"Group with ID[{groupId}] not found", ResultStatus.NotFound);

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
                return ApiResult.FailureResult($"Adding User with ID[{userId}] not found", ResultStatus.NotFound);

            await _repo.AddMemberAsync(userId, groupId);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> CreateAsync(PostChatDto groupPostDto)
        {
            var user = await _userRepo.GetByIdAsync(groupPostDto.OwnerId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{groupPostDto.OwnerId}] not found", ResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(groupPostDto?.Name))
                return ApiResult.FailureResult("Group name is required", ResultStatus.Failure);

            var chat = new Chat
            {
                Type = ChatType.Group,
                PrivacyType = ChatPrivacyType.Private
            };

            var chatId = await _chatRepo.CreateAsync(chat);

            var group = new Group
            {
                Id = chatId,
                CreatedById = user.Id,
                Description = groupPostDto.Description,
                Name = groupPostDto.Name,
                Members = new List<GroupMember>
                {
                    new GroupMember{ UserId = user.Id }
                }
            };

            if (groupPostDto.Avatar is not null)
            {
                if (groupPostDto.Avatar.FileName != null && groupPostDto.Avatar.Content != null && groupPostDto.Avatar.ContentType != null)
                {
                    group.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, groupPostDto.Avatar.FileName, groupPostDto.Avatar.Content, groupPostDto.Avatar.ContentType);
                }
            }

            await _repo.CreateAsync(group);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteAsync(Guid groupId)
        {
            var group = await _repo.GetByIdAsync(groupId);

            if (group is null)
            {
                return ApiResult.FailureResult($"Group with ID[{groupId}] not found");
            }

            await _repo.DeleteAsync(group);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> DeleteMemberAsync(Guid userId, Guid groupId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with ID[{userId}] not found");
            }

            var chat = await _chatRepo.GetByIdAsync(groupId);

            if (chat is null)
            {
                return ApiResult.FailureResult($"Group with ID[{groupId}] not found");
            }

            if (chat.Type != ChatType.Group)
            {
                return ApiResult.FailureResult($"Can't delete from Group TYPE[{chat.Type}]");
            }

            var groupMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userId
            };

            await _repo.DeleteMemberAsync(groupMember);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<List<SearchChatResponseDto>?>> SearchAsync(string searchTerm)
        {
            var results = await _repo.SearchAsync(searchTerm);

            var modeledResults = results
                .Select(r => new SearchChatResponseDto
                {
                    ChatId = r.Id,
                    AvatarUrl = r.AvatarUrl,
                    DisplayName = r.Name,
                    ChatType = ChatType.Group
                })
                .ToList();

            return ApiResult<List<SearchChatResponseDto>>.SuccessResult(modeledResults);
        }

        public async Task<ApiResult> UpdateAsync(Guid groupId, PostChatDto updateChatDto)
        {
            var group = await _repo.GetByIdAsync(groupId);

            if (group is null)
            {
                return ApiResult.FailureResult($"Group with ID[{groupId}] not found");
            }

            group.Name = updateChatDto.Name;
            group.Description = updateChatDto.Description;

            await _repo.UpdateAsync(group);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<List<UserChatResponseDto>>> GetUserSubscribedAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<List<UserChatResponseDto>>.FailureResult($"User with ID[{userId}] not found");
            }

            var groups = await _repo.GetUserParticipatedGroupsAsync(userId);

            var modeledGroups = new List<UserChatResponseDto>();

            foreach (var group in groups)
            {
                var notificationsCount = await _notificationRepo.GetUserChatNotificationsCountAsync(userId, group.Id);
                var lastMessage = await _messageRepo.GetLastMessageAsync(group.Id);
                var lastUserSendedMessage = await _messageRepo.GetUserLastSendedMessageAsync(userId, group.Id);

                var modeledGroup = new UserChatResponseDto
                {
                    Id = group.Id,
                    AvatarUrl = group.AvatarUrl,
                    LastMessage = new LastMessageResponseDto
                    {
                        Content = lastMessage.Content,
                        FileUrl = lastMessage.FileUrl,
                        SenderUsername = lastMessage.Sender.Username,
                        SentAt = lastMessage.SentAt
                    },
                    Name = group.Name,
                    NotificationsCount = notificationsCount,
                    Type = ChatType.Channel,
                    UserLastMessage = lastUserSendedMessage?.SentAt
                };

                modeledGroups.Add(modeledGroup);
            }

            return ApiResult<List<UserChatResponseDto>>.SuccessResult(modeledGroups);
        }
    }
}
