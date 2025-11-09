
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Reactions;
using Simpchat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IReactionService
    {
        Task<Result<List<GetAllReactionDto>>> GetAllAsync();
        Task<Result<Guid>> CreateAsync(PostReactionDto postReactionDto, UploadFileRequest? uploadFileRequest);
        Task<Result> UpdateAsync(Guid reactionId, UpdateReactionDto updateReactionDto, UploadFileRequest? uploadFileRequest);
        Task<Result> DeleteAsync(Guid reactionId);
        Task<Result<GetByIdReactionDto>> GetByIdAsync(Guid reactionId);
    }
}
