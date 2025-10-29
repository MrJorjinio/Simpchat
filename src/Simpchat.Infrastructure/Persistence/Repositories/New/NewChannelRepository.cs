using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Repository;
using Simpchat.Application.Interfaces.Repositories.New;
using Simpchat.Domain.Entities.Channels;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;

namespace Simpchat.Infrastructure.Persistence.Repositories.New
{
    public class NewChannelRepository : INewChannelRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public NewChannelRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddSubscriberAsync(Guid userId, Guid channelId)
        {
            await _dbContext.ChannelsSubscribers.AddAsync(new ChannelSubscriber { UserId = userId, ChannelId = channelId });
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Guid> CreateAsync(Channel entity)
        {
            await _dbContext.Channels.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteAsync(Channel entity)
        {
            _dbContext.Channels.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteSubscriberAsync(ChannelSubscriber channelSubscriber)
        {
            _dbContext.ChannelsSubscribers.Remove(channelSubscriber);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Channel>?> GetAllAsync()
        {
            return await _dbContext.Channels.ToListAsync();
        }

        public async Task<Channel?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Channels
                .Include(c => c.Subscribers)
                    .ThenInclude(s => s.User)
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Channel>> GetUserSubscribedChannelsAsync(Guid userId)
        {
            return await _dbContext.ChannelsSubscribers
                .Include(cs => cs.Channel)
                .Where(cs => cs.UserId == userId)
                .Select(cs => cs.Channel)
                .ToListAsync();
        }

        public async Task<List<Channel>?> SearchAsync(string term)
        {
            return await _dbContext.Channels
                .Where(c => EF.Functions.Like(c.Name, $"%{term}"))
                .ToListAsync();
        }

        public async Task UpdateAsync(Channel entity)
        {
            _dbContext.Channels.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
