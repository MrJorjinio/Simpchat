using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Repositories.Old;
using Simpchat.Infrastructure.Identity;

namespace Simpchat.Infrastructure.Persistence.Repositories.Old
{
    internal class GlobalPermissionRepository : IGlobalPermissionRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public GlobalPermissionRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(GlobalPermission permission)
        {
            await _dbContext.GlobalPermissions.AddAsync(permission);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(GlobalPermission permission)
        {
            _dbContext.GlobalPermissions.Remove(permission);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GlobalPermission?> GetByIdAsync(Guid id)
        {
            return await _dbContext.GlobalPermissions.FindAsync(id);
        }

        public async Task<GlobalPermission?> GetByNameAsync(string name)
        {
            return await _dbContext.GlobalPermissions.FirstOrDefaultAsync(gp => gp.Name == name);
        }

        public async Task UpdateAsync(GlobalPermission permission)
        {
            _dbContext.GlobalPermissions.Update(permission);
            await _dbContext.SaveChangesAsync();
        }    }
}
