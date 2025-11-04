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
    public class GlobalRoleRepository : IGlobalRoleRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public GlobalRoleRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(GlobalRole entity)
        {
            _dbContext.GlobalRoles.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(GlobalRole entity)
        {
            _dbContext.GlobalRoles.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<GlobalRole>?> GetAllAsync()
        {
            return await _dbContext.GlobalRoles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GlobalRole?> GetByIdAsync(Guid id)
        {
            return await _dbContext.GlobalRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<GlobalRole?> GetByNameAsync(string name)
        {
            return await _dbContext.GlobalRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task UpdateAsync(GlobalRole entity)
        {
            _dbContext.GlobalRoles.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
