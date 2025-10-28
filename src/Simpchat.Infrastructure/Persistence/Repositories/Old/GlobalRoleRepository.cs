using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Repositories.Old;
using Simpchat.Infrastructure.Identity;

namespace Simpchat.Infrastructure.Persistence.Repositories.Old
{
    internal class GlobalRoleRepository : IGlobalRoleRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public GlobalRoleRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(GlobalRole role)
        {
            await _dbContext.GlobalRoles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(GlobalRole role)
        {
            _dbContext.GlobalRoles.Remove(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GlobalRole?> GetByIdAsync(Guid id)
        {
            return await _dbContext.GlobalRoles.FindAsync(id);
        }

        public async Task<GlobalRole?> GetByNameAsync(string name)
        {
            return await _dbContext.GlobalRoles.FirstOrDefaultAsync(gr => gr.Name == name);
        }

        public async Task UpdateAsync(GlobalRole role)
        {
            _dbContext.Update(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AssignTo(Guid roleId, Guid permissionId)
        {
            await _dbContext.GlobalRolesPermissions.AddAsync(new GlobalRolePermission { RoleId = roleId, PermissionId = permissionId });
            await _dbContext.SaveChangesAsync();
        }

        public async Task AssignPermission(Guid roleId, Guid permissionId)
        {
            
        }
    }
}
