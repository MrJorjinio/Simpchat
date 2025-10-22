using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public NotificationRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Message message, Chat chat, User user)
        {
            if (chat.Type == ChatType.Conversation)
            {
                _dbContext.Notifications.Add(new Notification { MessageId = message.Id, ReceiverId = chat.Conversation.UserId1 == message.Id ? chat.Conversation.UserId2 : chat.Conversation.UserId1 });
            }
            else if (chat.Type == ChatType.Group)
            {
                foreach (var member in chat.Group.Members.Where(m => m.UserId != user.Id))
                {
                    _dbContext.Add(new Notification { MessageId = message.Id, ReceiverId = member.UserId });
                }
            }
            else
            {
                foreach (var subscriber in chat.Channel.Subscribers.Where(s => s.UserId != user.Id))
                {
                    _dbContext.Add(new Notification { MessageId = message.Id, ReceiverId = subscriber.UserId });
                }
            }

            _dbContext.SaveChangesAsync();
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            return await _dbContext.Notifications
                .Include(n => n.Receiver)
                .Include(n => n.Message)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Notification> GetByIdAsync(Guid messageId, Guid userId)
        {
            return await _dbContext.Notifications
                .Include(n => n.Receiver)
                .Include(n => n.Message)
                .FirstOrDefaultAsync(n => n.MessageId == messageId && n.ReceiverId == userId);
        }

        public async Task UpdateAsync(Notification notification)
        {
            _dbContext.Notifications.Update(notification);
            await _dbContext.SaveChangesAsync();
        }
    }
}
