using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Repository;
using Simpchat.Application.Interfaces.Repositories.New;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;

namespace Simpchat.Infrastructure.Persistence.Repositories.New
{
    public class NewNotificationRepository : INewNotificationRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public NewNotificationRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(Notification entity)
        {
            await _dbContext.Notifications.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteAsync(Notification entity)
        {
            _dbContext.Notifications.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Notification>?> GetAllAsync()
        {
            return await _dbContext.Notifications
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Notifications
                .Include(n => n.Message)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public Task<int> GetUserChatNotificationsCountAsync(Guid userId, Guid chatId)
        {
            return _dbContext.Notifications
                .Include(n => n.Message)
                .Where(n => n.ReceiverId == userId && n.Message.ChatId == chatId && n.IsSeen == false)
                .CountAsync();
        }

        public async Task UpdateAsync(Notification entity)
        {
            _dbContext.Notifications.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
