using Simpchat.Application.Common.Interfaces.External.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Chats.Post;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Application.Common.Models.Pagination;
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

namespace Simpchat.Application.Features.Groups
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

        public async Task<ApiResult> CreateAsync(Guid userId, ChatPostDto chatPostDto, FileUploadRequest? avatar)
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

            group.AvatarUrl = await _fileStorageService.UploadFileAsync(BucketName, avatar.FileName, avatar.Content, avatar.ContentType);

            await _groupRepository.CreateAsync(group);

            return ApiResult.SuccessResult();
        }
    }
}
