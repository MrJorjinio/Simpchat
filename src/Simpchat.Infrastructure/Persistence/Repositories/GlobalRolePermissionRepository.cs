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
    public class GlobalRolePermissionRepository : IGlobalRolePermissionRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public GlobalRolePermissionRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(GlobalRolePermission entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(GlobalRolePermission entity)
        {
            _dbContext.GlobalRolesPermissions.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<GlobalRolePermission>?> GetAllAsync()
        {
            return await _dbContext.GlobalRolesPermissions
                .ToListAsync();
        }

        public async Task<GlobalRolePermission?> GetByIdAsync(Guid id)
        {
            return await _dbContext.GlobalRolesPermissions
                .Include(grp => grp.Role)
                .Include(grp => grp.Permission)
                .FirstOrDefaultAsync(grp => grp.Id == id);
        }

        public async Task<Guid> GetIdAsync(Guid roleId, Guid permissionId)
        {
            var rolePermission = await _dbContext.GlobalRolesPermissions
                .FirstOrDefaultAsync(grp => grp.RoleId == roleId && grp.PermissionId == permissionId);

            return rolePermission.Id;
        }

        public async Task UpdateAsync(GlobalRolePermission entity)
        {
            _dbContext.GlobalRolesPermissions.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
