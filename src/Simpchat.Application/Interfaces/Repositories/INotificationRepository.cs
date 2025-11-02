using Simpchat.Application.Common.Repository;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<int> GetUserChatNotificationsCountAsync(Guid userId, Guid chatId);
        Task<bool> GetMessageSeenStatusAsync(Guid messageId);
        Task<bool> CheckIsNotSeenAsync(Guid messageId, Guid userId);
        Task<Guid> GetIdAsync(Guid messageId, Guid userId);
    }
}
