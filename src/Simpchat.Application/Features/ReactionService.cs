using FluentValidation;
using Simpchat.Application.Errors;
using Simpchat.Application.Interfaces.File;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;

using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Reactions;
using Simpchat.Application.Validators;
using Simpchat.Domain.Entities;
using Simpchat.Shared.Models;
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
        private readonly IValidator<PostReactionDto> _postReactionValidator;
        private readonly IValidator<UpdateReactionDto> _updateReactionValidator;
        private const string BucketName = "reactions-images";

        public ReactionService(
            IReactionRepository repo,
            IFileStorageService fileStorageService,
            IValidator<PostReactionDto> postReactionValidator,
            IValidator<UpdateReactionDto> updateReactionValidator)
        {
            _repo = repo;
            _fileStorageService = fileStorageService;
            _postReactionValidator = postReactionValidator;
            _updateReactionValidator = updateReactionValidator;
        }

        public async Task<Result<Guid>> CreateAsync(PostReactionDto postReactionDto, UploadFileRequest? uploadFileRequest)
        {
            var validationResult = await _postReactionValidator.ValidateAsync(postReactionDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Result.Failure<Guid>(ApplicationErrors.Validation.Failed, errors);
            }

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

            return reaction.Id;
        }

        public async Task<Result> DeleteAsync(Guid reactionId)
        {
            var reaction = await _repo.GetByIdAsync(reactionId);

            if (reaction is null)
            {
                return Result.Failure(ApplicationErrors.Reaction.IdNotFound);
            }

            await _repo.DeleteAsync(reaction);

            return Result.Success();
        }

        public async Task<Result<List<GetAllReactionDto>>> GetAllAsync()
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

            return reactionModels;
        }

        public async Task<Result<GetByIdReactionDto>> GetByIdAsync(Guid reactionId)
        {
            var reaction = await _repo.GetByIdAsync(reactionId);

            if (reaction is null)
            {
                return Result.Failure<GetByIdReactionDto>(ApplicationErrors.Reaction.IdNotFound);
            }

            var reactionModel = new GetByIdReactionDto
            {
                Name = reaction.Name,
                ImageUrl = reaction.ImageUrl
            };

            return reactionModel;
        }

        public async Task<Result> UpdateAsync(Guid reactionId, UpdateReactionDto updateReactionDto, UploadFileRequest? uploadFileRequest)
        {
            var validationResult = await _updateReactionValidator.ValidateAsync(updateReactionDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Result.Failure<Guid>(ApplicationErrors.Validation.Failed, errors);
            }

            var reaction = await _repo.GetByIdAsync(reactionId);

            if (reaction is null)
            {
                return Result.Failure(ApplicationErrors.Reaction.IdNotFound);
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

            return Result.Success();
        }
    }
}
