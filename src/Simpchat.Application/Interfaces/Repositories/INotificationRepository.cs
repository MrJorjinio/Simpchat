using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Message message, Chat chat, User user);
        Task UpdateAsync(Notification notification);
        Task<Notification> GetByIdAsync(Guid id);
        Task<Notification> GetByIdAsync(Guid messageId, Guid userId);
    }
}
