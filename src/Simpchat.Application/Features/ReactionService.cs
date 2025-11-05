using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Reactions;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Simpchat.Application.Features
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _repo;
        private readonly IFileStorageService _fileStorageService;
        private const string BucketName = "reactions-images";

        public ReactionService(IReactionRepository repo, IFileStorageService fileStorageService)
        {
            _repo = repo;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResult<Guid>> CreateAsync(PostReactionDto postReactionDto, UploadFileRequest? uploadFileRequest)
        {
            var reaction = new Reaction
            {
                Name = postReactionDto.Name
            };

            if (uploadFileRequest is not null)
            {
                if (uploadFileRequest.FileName != null && uploadFileRequest.Content != null && uploadFileRequest.ContentType != null)
                {
                    reaction.ImageUrl = await _fileStorageService.UploadFileAsync(BucketName, uploadFileRequest.FileName, uploadFileRequest.Content, uploadFileRequest.ContentType);
                }
            }

            await _repo.CreateAsync(reaction);

            return ApiResult<Guid>.SuccessResult(reaction.Id);
        }

        public async Task<ApiResult> DeleteAsync(Guid reactionId)
        {
            var reaction = await _repo.GetByIdAsync(reactionId);

            if (reaction is null)
            {
                return ApiResult.FailureResult($"Reaction with ID[{reactionId} not found");
            }

            await _repo.DeleteAsync(reaction);

            return ApiResult.SuccessResult();
        }

        public async Task<ApiResult<List<GetAllReactionDto>>> GetAllAsync()
        {
            var reactions = await _repo.GetAllAsync();

            var reactionModels = new List<GetAllReactionDto>();

            foreach (var reaction in reactions)
            {
                var reactionModel = new GetAllReactionDto
                {
                    ImageUrl = reaction.ImageUrl,
                    Name = reaction.Name
                };

                reactionModels.Add(reactionModel);
            }

            return ApiResult<List<GetAllReactionDto>>.SuccessResult(reactionModels);
        }

        public async Task<ApiResult<GetByIdReactionDto>> GetByIdAsync(Guid reactionId)
        {
            var reaction = await _repo.GetByIdAsync(reactionId);

            if (reaction is null)
            {
                return ApiResult<GetByIdReactionDto>.FailureResult($"Reaction with ID[{reactionId} not found");
            }

            var reactionModel = new GetByIdReactionDto
            {
                Name = reaction.Name,
                ImageUrl = reaction.ImageUrl
            };

            return ApiResult<GetByIdReactionDto>.SuccessResult(reactionModel);
        }

        public async Task<ApiResult> UpdateAsync(Guid reactionId, UpdateReactionDto updateReactionDto, UploadFileRequest? uploadFileRequest)
        {
            var reaction = await _repo.GetByIdAsync(reactionId);

            if (reaction is null)
            {
                return ApiResult.FailureResult($"Reaction with ID[{reactionId} not found");
            }

            reaction.Name = updateReactionDto.Name;

            if (uploadFileRequest is not null)
            {
                if (uploadFileRequest.FileName != null && uploadFileRequest.Content != null && uploadFileRequest.ContentType != null)
                {
                    reaction.ImageUrl = await _fileStorageService.UploadFileAsync(BucketName, uploadFileRequest.FileName, uploadFileRequest.Content, uploadFileRequest.ContentType);
                }
            }

            await _repo.UpdateAsync(reaction);

            return ApiResult.SuccessResult();
        }
    }
}
