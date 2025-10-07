using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GlobalPermissions;
using SimpchatWeb.Services.Interfaces.DataInserter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpchatWeb.Services.DataInserter
{
    public class DataInserter : IGlobalDataInserter, IChatDataInserter
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IMapper _mapper;
        public DataInserter(
            SimpchatDbContext dbContext,
            IMapper mapper
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddPermissionToRoleAsync(string roleName, string permissionName)
        {
            var role = await _dbContext.GlobalRoles.FirstOrDefaultAsync(gr => gr.Name == roleName);
            if (role is null) return;

            var permission = await _dbContext.GlobalPermissions.FirstOrDefaultAsync(gp => gp.Name == permissionName);
            if (permission is null) return;

            var dbRolePermission = await _dbContext.GlobalRolesPermissions
                .FirstOrDefaultAsync(grp => grp.RoleId == role.Id && grp.PermissionId == permission.Id);
            if (dbRolePermission is not null) return;

            var rolePermission = new GlobalRolePermission()
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            };

            await _dbContext.GlobalRolesPermissions.AddAsync(rolePermission);
            await _dbContext.SaveChangesAsync();
        }

        public async Task InsertSysGroupPermissionsAsync()
        {
            var sysChatPermissions = Enum.GetNames<ChatPermissionType>();

            var dbChatPermissions = await _dbContext.ChatPermissions
                .Select(x => x.Name)
                .ToListAsync();

            var newPermissions = sysChatPermissions
                .Where(p => !dbChatPermissions.Contains(p))
                .Select(p => new ChatPermission { Name = p })
                .ToList();

            if (newPermissions.Count > 0)
            {
                await _dbContext.ChatPermissions.AddRangeAsync(newPermissions);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task InsertSysPermissionsAsync()
        {
            var sysGlobalPermissions = Enum.GetNames<GlobalPermissionType>();

            var dbGlobalPermissions = await _dbContext.GlobalPermissions
                .Select(x => x.Name)
                .ToListAsync();

            var newPermissions = sysGlobalPermissions
                .Where(p => !dbGlobalPermissions.Contains(p))
                .Select(p => new GlobalPermission { Name = p, Description = $"for {p}" })
                .ToList();

            if (newPermissions.Count > 0)
            {
                await _dbContext.GlobalPermissions.AddRangeAsync(newPermissions);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task InsertSysRolesAsync()
        {
            var sysGlobalRoles = Enum.GetNames<GlobalRoleType>();

            var dbGlobalRoles = await _dbContext.GlobalRoles
                .Select(x => x.Name)
                .ToListAsync();

            var newRoles = sysGlobalRoles
                .Where(r => !dbGlobalRoles.Contains(r))
                .Select(r => new GlobalRole { Name = r, Description = $"for {r}" })
                .ToList();

            if (newRoles.Count > 0)
            {
                await _dbContext.GlobalRoles.AddRangeAsync(newRoles);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertPermissionAsync(GlobalPermissionDto permission)
        {
            var globalDbPermission = await _dbContext.GlobalPermissions
                .FirstOrDefaultAsync(p => p.Name == permission.Name);

            bool isDescriptionGiven = !string.IsNullOrEmpty(permission.Description);

            if (globalDbPermission is null)
            {
                var _permission = new GlobalPermission()
                {
                    Name = permission.Name,
                    Description = isDescriptionGiven ? permission.Description : $"for {permission.Name}"
                };
                await _dbContext.GlobalPermissions.AddAsync(_permission);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                globalDbPermission = _mapper.Map(permission, globalDbPermission);
                globalDbPermission.Description = isDescriptionGiven ? permission.Description : $"for {permission.Name}";
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
