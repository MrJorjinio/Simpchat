using FluentValidation;
using Simpchat.Application.Errors;
using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;

using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Messages;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;
using Simpchat.Shared.Models;

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
        private readonly IValidator<UpdateChatDto> _updateValidator;
        private readonly IValidator<PostChatDto> _createValidator;
        private const string BucketName = "groups-avatars";

        public GroupService(
            IGroupRepository repo,
            IUserRepository userRepo,
            IChatRepository chatRepo,
            IFileStorageService fileStorageService,
            INotificationRepository notificationRepository,
            IMessageRepository messageRepository,
            IValidator<UpdateChatDto> updateValidator,
            IValidator<PostChatDto> createValidator)
        {
            _repo = repo;
            _userRepo = userRepo;
            _chatRepo = chatRepo;
            _fileStorageService = fileStorageService;
            _notificationRepo = notificationRepository;
            _messageRepo = messageRepository;
            _updateValidator = updateValidator;
            _createValidator = createValidator;
        }

        public async Task<Result> AddMemberAsync(Guid groupId, Guid userId)
        {
            var group = await _repo.GetByIdAsync(groupId);

            if (group is null)
                return Result.Failure(ApplicationErrors.Chat.IdNotFound);

            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
                return Result.Failure(ApplicationErrors.User.IdNotFound);

            await _repo.AddMemberAsync(userId, groupId);

            return Result.Success();
        }

        public async Task<Result<Guid>> CreateAsync(PostChatDto groupPostDto, UploadFileRequest? avatar)
        {
            var validationResult = await _createValidator.ValidateAsync(groupPostDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Result.Failure<Guid>(ApplicationErrors.Validation.Failed, errors);
            }

            var user = await _userRepo.GetByIdAsync(groupPostDto.OwnerId);

            if (user is null)
            {
                return Result.Failure<Guid>(ApplicationErrors.User.IdNotFound);
            }

            var chat = new Chat
            {
                Type = ChatTypes.Group,
                PrivacyType = ChatPrivacyTypes.Private
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

            return group.Id;
        }

        public async Task<Result> DeleteAsync(Guid groupId)
        {
            var group = await _repo.GetByIdAsync(groupId);

            if (group is null)
            {
                return Result.Failure(ApplicationErrors.Chat.IdNotFound);
            }

            await _repo.DeleteAsync(group);

            return Result.Success();
        }

        public async Task<Result> DeleteMemberAsync(Guid userId, Guid groupId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure(ApplicationErrors.User.IdNotFound);
            }

            var chat = await _chatRepo.GetByIdAsync(groupId);

            if (chat is null)
            {
                return Result.Failure(ApplicationErrors.Chat.IdNotFound);
            }

            if (chat.Type != ChatTypes.Group)
            {
                return Result.Failure(ApplicationErrors.Chat.NotValidChatType);
            }

            var groupMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userId
            };

            await _repo.DeleteMemberAsync(groupMember);

            return Result.Success();
        }

        public async Task<Result<List<SearchChatResponseDto>?>> SearchAsync(string searchTerm)
        {
            var results = await _repo.SearchAsync(searchTerm);

            var modeledResults = results
                .Select(r => new SearchChatResponseDto
                {
                    ChatId = r.Id,
                    AvatarUrl = r.AvatarUrl,
                    DisplayName = r.Name,
                    ChatType = ChatTypes.Group
                })
                .ToList();

            return modeledResults;
        }

        public async Task<Result> UpdateAsync(Guid groupId, UpdateChatDto updateChatDto, UploadFileRequest? avatar)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateChatDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Result.Failure<Guid>(ApplicationErrors.Validation.Failed, errors);
            }

            var group = await _repo.GetByIdAsync(groupId);

            if (group is null)
            {
                return Result.Failure(ApplicationErrors.Chat.IdNotFound);
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

            return Result.Success();
        }

        public async Task<Result<List<UserChatResponseDto>>> GetUserParticipatedAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure<List<UserChatResponseDto>>(ApplicationErrors.User.IdNotFound);
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
                    Type = ChatTypes.Channel,
                    UserLastMessage = lastUserSendedMessage?.SentAt
                };

                modeledGroups.Add(modeledGroup);
            }

            modeledGroups.OrderByDescending(mg => (DateTimeOffset?)mg.LastMessage.SentAt ?? DateTimeOffset.MinValue);

            return modeledGroups;
        }
    }
}
