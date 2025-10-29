using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories.New
{
    public interface INewMessageRepository : IBaseRepository<Message>
    {
        Task<Message?> GetLastMessageAsync(Guid chatId);
        Task<Message?> GetUserLastSendedMessageAsync(Guid userId, Guid chatId);
    }
}
