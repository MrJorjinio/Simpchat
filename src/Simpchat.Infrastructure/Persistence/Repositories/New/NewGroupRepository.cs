using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Repository;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories.New
{
    public class NewGroupRepository : IBaseRepository<Group>, ISearchableRepository<Group>
    {
        private readonly SimpchatDbContext _dbContext;

        public NewGroupRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(Group entity)
        {
            await _dbContext.Groups.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteAsync(Group entity)
        {
            _dbContext.Groups.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Group>?> GetAllAsync()
        {
            return await _dbContext.Groups.ToListAsync();
        }

        public async Task<Group?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Groups
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .Include(g => g.Owner)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Group>?> SearchAsync(string term)
        {
            return await _dbContext.Groups
                .Where(g => EF.Functions.Like(g.Name, $"%{term}"))
                .ToListAsync();
        }

        public async Task UpdateAsync(Group entity)
        {
            _dbContext.Groups.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
