
using Simpchat.Application.Models.Reactions;
using Simpchat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IMessageReactionService
    {
        Task<Result<Guid>> CreateAsync(Guid messageId, Guid reactionId, Guid userId);
        Task<Result> DeleteAsync(Guid messageId, Guid userId);
    }
}