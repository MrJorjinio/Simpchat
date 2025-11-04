using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Files;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;

namespace Simpchat.Application.Features
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationRepository _notificationRepo;
        private readonly IMessageRepository _messageRepo;
        private const string BucketName = "groups-avatars";

        public GroupService(
            IGroupRepository repo,
            IUserRepository userRepo,
            IChatRepository chatRepo,
            IFileStorageService fileStorageService,
            INotificationRepository notificationRepository,
            IMessageRepository messageRepository)
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

        public async Task<ApiResult<Guid>> CreateAsync(PostChatDto groupPostDto, UploadFileRequest? avatar)
        {
            var user = await _userRepo.GetByIdAsync(groupPostDto.OwnerId);

            if (user is null)
            {
                return ApiResult<Guid>.FailureResult($"User with ID[{groupPostDto.OwnerId}] not found", ResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(groupPostDto?.Name))
                return ApiResult<Guid>.FailureResult("Group name is required", ResultStatus.Failure);

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

            if (avatar is not null)
            {
                if (avatar.FileName != null && avatar.Content != null && avatar.ContentType != null)
                {
                    group.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);
                }
            }

            await _repo.CreateAsync(group);

            return ApiResult<Guid>.SuccessResult(group.Id);
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

        public async Task<ApiResult> UpdateAsync(Guid groupId, PutChatDto updateChatDto, UploadFileRequest? avatar)
        {
            var group = await _repo.GetByIdAsync(groupId);

            if (group is null)
            {
                return ApiResult.FailureResult($"Group with ID[{groupId}] not found");
            }

            group.Name = updateChatDto.Name;
            group.Description = updateChatDto.Description;

            if (avatar is not null)
            {
                if (avatar.FileName != null && avatar.Content != null && avatar.ContentType != null)
                {
                    group.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);
                }
            }

            await _repo.UpdateAsync(group);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<List<UserChatResponseDto>>> GetUserParticipatedAsync(Guid userId)
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

            modeledGroups.OrderByDescending(mg => (DateTimeOffset?)mg.LastMessage.SentAt ?? DateTimeOffset.MinValue);

            return ApiResult<List<UserChatResponseDto>>.SuccessResult(modeledGroups);
        }
    }
}
