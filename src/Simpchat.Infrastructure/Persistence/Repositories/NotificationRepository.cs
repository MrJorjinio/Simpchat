using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public NotificationRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Message message, User user)
        {
            await _dbContext.Notifications.AddAsync(new Notification { MessageId = message.Id, ReceiverId = user.Id, IsSeen = false });
            await _dbContext.SaveChangesAsync();
        }
    }
}
