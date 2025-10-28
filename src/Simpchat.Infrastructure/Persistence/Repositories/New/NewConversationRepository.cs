﻿using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Repository;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories.New
{
    public class NewConversationRepository : IBaseRepository<Conversation>
    {
        private readonly SimpchatDbContext _dbContext;

        public NewConversationRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(Conversation entity)
        {
            await _dbContext.Conversations.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteAsync(Conversation entity)
        {
            _dbContext.Conversations.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Conversation>?> GetAllAsync()
        {
            return await _dbContext.Conversations.ToListAsync();
        }

        public async Task<Conversation?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Conversations.FindAsync(id);
        }

        public async Task UpdateAsync(Conversation entity)
        {
            _dbContext.Conversations.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Guid?> GetConversationBetweenAsync(Guid userId1, Guid userId2)
        {
            var conversation = await _dbContext.Conversations
                .FirstOrDefaultAsync(c =>
                (c.UserId1 == userId1 && c.UserId2 == userId2)
                ||
                (c.UserId1 == userId2 && c.UserId2 == userId1)
                );

            return conversation?.Id;
        }
    }
}
