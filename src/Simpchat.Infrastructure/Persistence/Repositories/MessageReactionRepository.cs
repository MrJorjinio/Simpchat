using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class MessageReactionRepository : IMessageReactionRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public MessageReactionRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(MessageReaction entity)
        {
            await _dbContext.MessagesReactions.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(MessageReaction entity)
        {
            _dbContext.MessagesReactions.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<MessageReaction>?> GetAllAsync()
        {
            return await _dbContext.MessagesReactions.ToListAsync();
        }

        public async Task<MessageReaction?> GetByIdAsync(Guid id)
        {
            return await _dbContext.MessagesReactions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Guid?> GetIdAsync(Guid userId, Guid messageId)
        {
            var userReaction = await _dbContext.MessagesReactions
                .FirstOrDefaultAsync(mr => mr.UserId == userId && mr.MessageId == messageId);

            return userReaction?.Id;
        }

        public async Task<List<MessageReaction>?> GetMessageReactionAsync(Guid messageId)
        {
            return await _dbContext.MessagesReactions
                .Include(mr => mr.Reaction)
                .Where(mr => mr.MessageId == messageId)
                .ToListAsync();
        }

        public async Task UpdateAsync(MessageReaction entity)
        {
            _dbContext.MessagesReactions.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
