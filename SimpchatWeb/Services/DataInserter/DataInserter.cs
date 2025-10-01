using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models;
using SimpchatWeb.Services.GlobalEnums;
using SimpchatWeb.Services.Interfaces.DataInserter;

namespace SimpchatWeb.Services.DataInserter
{
    public class DataInserter : IGlobalDataInserter, IGroupDataInserter
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
        public void AddPermissionToRole(string roleName, string permissionName)
        {
            var role = _dbContext.GlobalRoles.FirstOrDefault(gr => gr.Name == roleName);
            if (role is null)
            {
                return;
            }

            var permission = _dbContext.GlobalPermissions.FirstOrDefault(gp => gp.Name == permissionName);
            if (permission is null)
            {
                return;
            }

            var dbRolePermission = _dbContext.GlobalRolesPermissions.Where(grp => grp.RoleId == role.Id && grp.PermissionId == permission.Id);
            if (dbRolePermission is not null)
            {
                return;
            }

            var rolePermission = new GlobalRolePermission()
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            };

            _dbContext.GlobalRolesPermissions.Add(rolePermission);
            _dbContext.SaveChanges();
        }

        public void InsertSysGroupPermissions()
        {
            var sysGroupPermissions = Enum.GetNames<GroupPermissions>();

            var dbGroupPermissions = _dbContext.GroupPermissions
                .Select(x => x.Name)
                .ToHashSet();

            var newPermissions = sysGroupPermissions
                .Where(p => !dbGroupPermissions.Contains(p))
                .Select(p => new GroupPermission { Name = p })
                .ToList();

            if (newPermissions.Count > 0)
            {
                _dbContext.GroupPermissions.AddRange(newPermissions);
                _dbContext.SaveChanges();
            }
        }

        public void InsertSysPermissions()
        {
            var sysGlobalPermissions = Enum.GetNames<GlobalPermissions>();

            var dbGlobalPermissions = _dbContext.GlobalPermissions
                .Select(x => x.Name)
                .ToHashSet();

            var newPermissions = sysGlobalPermissions
                .Where(p => !dbGlobalPermissions.Contains(p))
                .Select(p => new GlobalPermission { Name = p, Description = $"for {p}" })
                .ToList();

            if (newPermissions.Count > 0)
            {
                _dbContext.GlobalPermissions.AddRange(newPermissions);
                _dbContext.SaveChanges();
            }
        }

        public void InsertSysRoles()
        {
            var sysGlobalRoles = Enum.GetNames<GlobalRoles>();

            var dbGlobalRoles = _dbContext.GlobalRoles
                .Select(x => x.Name)
                .ToHashSet();

            var newRoles = sysGlobalRoles
                .Where(r => !dbGlobalRoles.Contains(r))
                .Select(r => new GlobalRole { Name = r, Description = $"for {r}" })
                .ToList();

            if (newRoles.Count > 0)
            {
                _dbContext.GlobalRoles.AddRange(newRoles);
                _dbContext.SaveChanges();
            }
        }

        public void UpsertPermission(GlobalPermissionDto permission)
        {
            var globalDbPermission = _dbContext.GlobalPermissions
                .FirstOrDefault(p => p.Name == permission.Name);

            bool isDescriptionGiven = permission.Description != string.Empty;

            if (globalDbPermission is null)
            {
                var _permission = new GlobalPermission()
                {
                    Name = $"{permission.Name}"
                };
                if (isDescriptionGiven is true)
                {
                    _permission.Description = permission.Description;
                }
                _permission.Description = $"{permission.Description}";
                _dbContext.GlobalPermissions.Add(_permission);
                _dbContext.SaveChanges();
            }
            else
            {
                globalDbPermission = _mapper.Map(permission, globalDbPermission);
                if (isDescriptionGiven is true)
                {
                    globalDbPermission.Description = permission.Description;
                }
                else
                {
                    globalDbPermission.Description = $"for {permission.Name}";
                }
                _dbContext.SaveChanges();
            }
        }
    }
}
