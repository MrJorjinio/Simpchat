using AutoMapper;
using Simpchat.Application.Interfaces.External.FileStorage;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.ApiResults.Enums;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Users.GetById;
using Simpchat.Application.Models.Users.Response;
using Simpchat.Application.Models.Users.Update;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.Users.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "profile-pics";

        public UserService(IUserRepository userRepository, IFileStorageService fileStorageService)
        {
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResult<GetByIdUserDto>> GetByIdAsync(Guid id, Guid currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(id, currentUserId);

            if (user is null)
            {
                return ApiResult<GetByIdUserDto>.FailureResult($"User with id[{id}] not found", ResultStatus.NotFound);
            }

            return ApiResult<GetByIdUserDto>.SuccessResult(user);
        }

        public async Task<ApiResult<ICollection<SearchChatResponseDto>>?> SearchByUsernameAsync(string searchTerm, Guid currentUserId)
        {
            if (await _userRepository.GetByIdAsync(currentUserId) is null)
            {
                ApiResult.FailureResult($"User with ID[{currentUserId}] not found", ResultStatus.NotFound);
            }

            var users = await _userRepository.SearchByUsernameAsync(searchTerm, currentUserId);
            return ApiResult<ICollection<SearchChatResponseDto>>.SuccessResult(users);
        }

        public async Task<ApiResult<UserResponseDto>> SetLastSeenAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult<UserResponseDto>.FailureResult("User with id[{userId}] not found", ResultStatus.NotFound);
            }

            await _userRepository.UpdateAsync(user);

            return ApiResult<UserResponseDto>.SuccessResult(UserResponseDto.ConvertFromDomainObject(user));
        }

        public async Task<ApiResult> UpdateAvatarAsync(Guid currentUserId, UploadFileRequest fileUploadRequest)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with id[{currentUserId}] not found", ResultStatus.NotFound);
            }

            var avatarUrl = await _fileStorageService.UploadFileAsync(BucketName, fileUploadRequest.FileName, fileUploadRequest.Content, fileUploadRequest.ContentType);

            if (avatarUrl is null)
            {
                return ApiResult.FailureResult("Invalid file", ResultStatus.Failure);
            }

            user.AvatarUrl = avatarUrl;
            await _userRepository.UpdateAsync(user);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult> UpdateInfoAsync(Guid userId, UpdateUserInfoDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                return ApiResult.FailureResult($"User with id[{userId}] not found", ResultStatus.NotFound);
            }

            if (user.Username != dto.Username)
            {
                if(await _userRepository.GetByUsernameAsync(dto.Username) is null)
                {
                    user.Username = dto.Username;
                }
                else
                {
                    return ApiResult.FailureResult($"User with USERNAME[{dto.Username}] already exists", ResultStatus.Failure);
                }
            }
            if (user.Description != dto.Description)
            {
                user.Description = dto.Description;
            }

            await _userRepository.UpdateAsync(user);

            return ApiResult.SuccessResult();
        }
    }
}
