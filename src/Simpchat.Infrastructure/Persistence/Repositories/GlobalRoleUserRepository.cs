using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class GlobalRoleUserRepository : IGlobalRoleUserRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public GlobalRoleUserRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(GlobalRoleUser entity)
        {
            await _dbContext.UsersGlobalRoles.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(GlobalRoleUser entity)
        {
            _dbContext.UsersGlobalRoles.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<GlobalRoleUser>?> GetAllAsync()
        {
            return await _dbContext.UsersGlobalRoles.ToListAsync();
        }

        public async Task<GlobalRoleUser?> GetByIdAsync(Guid id)
        {
            return await _dbContext.UsersGlobalRoles.FindAsync(id);
        }

        public async Task<List<GlobalRole>> GetUserRolesAsync(Guid userId)
        {
            var roles = await _dbContext.UsersGlobalRoles
                .Where(ugr => ugr.UserId == userId)
                .Select(ugr => ugr.Role)
                .ToListAsync();

            return roles;
        }

        public async Task UpdateAsync(GlobalRoleUser entity)
        {
            _dbContext.UsersGlobalRoles.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
