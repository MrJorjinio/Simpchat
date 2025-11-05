using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IMessageReactionRepository : IBaseRepository<MessageReaction>
    {
        Task<Guid?> GetIdAsync(Guid userId, Guid messageId);
        Task<List<MessageReaction>?> GetMessageReactionAsync(Guid messageId);
    }
}
