using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Reactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IReactionService
    {
        Task<ApiResult<List<GetAllReactionDto>>> GetAllAsync();
        Task<ApiResult<Guid>> CreateAsync(PostReactionDto postReactionDto, UploadFileRequest? uploadFileRequest);
        Task<ApiResult> UpdateAsync(Guid reactionId, UpdateReactionDto updateReactionDto, UploadFileRequest? uploadFileRequest);
        Task<ApiResult> DeleteAsync(Guid reactionId);
        Task<ApiResult<GetByIdReactionDto>> GetByIdAsync(Guid reactionId);
    }
}
