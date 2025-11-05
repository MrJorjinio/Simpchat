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
    public class ReactionRepository : IReactionRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public ReactionRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(Reaction entity)
        {
            await _dbContext.Reactions.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(Reaction entity)
        {
            _dbContext.Reactions.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Reaction>?> GetAllAsync()
        {
            return await _dbContext.Reactions.ToListAsync();
        }

        public async Task<Reaction?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Reactions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Reaction entity)
        {
            _dbContext.Reactions.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
