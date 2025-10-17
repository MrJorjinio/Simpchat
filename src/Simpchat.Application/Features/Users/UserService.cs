using AutoMapper;
using Simpchat.Application.Common.Interfaces.External.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.ApiResults.Enums;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Application.Common.Models.Files;
using Simpchat.Application.Common.Models.Users;
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

        public async Task<ApiResult<UserGetByIdDto>> GetByIdAsync(Guid id, Guid currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(id, currentUserId);

            if (user is null)
            {
                return ApiResult<UserGetByIdDto>.FailureResult($"User with id[{id}] not found", ResultStatus.NotFound);
            }

            return ApiResult<UserGetByIdDto>.SuccessResult(user);
        }

        public async Task<ApiResult<ICollection<ChatSearchResponseDto>>?> SearchByUsernameAsync(string searchTerm, Guid currentUserId)
        {
            if (await _userRepository.GetByIdAsync(currentUserId) is null)
            {
                ApiResult.FailureResult($"User with ID[{currentUserId}] not found", ResultStatus.NotFound);
            }

            var users = await _userRepository.SearchByUsernameAsync(searchTerm, currentUserId);
            return ApiResult<ICollection<ChatSearchResponseDto>>.SuccessResult(users);
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

        public async Task<ApiResult<UserResponseDto>> UpdateProfileAsync(Guid currentUserId, FileUploadRequest fileUploadRequest)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);

            if (user is null)
            {
                return ApiResult<UserResponseDto>.FailureResult($"User with id[{currentUserId}] not found", ResultStatus.NotFound);
            }

            var profilePicUrl = await _fileStorageService.UploadFileAsync(BucketName, fileUploadRequest.FileName, fileUploadRequest.Content, fileUploadRequest.ContentType);

            if (profilePicUrl is null)
            {
                return ApiResult<UserResponseDto>.FailureResult("Invalid file", ResultStatus.Failure);
            }

            return ApiResult<UserResponseDto>.SuccessResult(UserResponseDto.ConvertFromDomainObject(user));
        }
    }
}
