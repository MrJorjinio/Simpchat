using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Reactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IMessageReactionService
    {
        Task<ApiResult<Guid>> CreateAsync(Guid messageId, Guid reactionId, Guid userId);
        Task<ApiResult> DeleteAsync(Guid messageId, Guid userId);
    }
}