using AutoMapper;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models;
using SimpchatWeb.Services.GlobalEnums;
using SimpchatWeb.Services.Interfaces;

namespace SimpchatWeb.Services.DataInserter
{
    public class DataInserter : IDataInserter
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IMapper _mapper;
        public DataInserter(SimpchatDbContext dbContext, IMapper mapper)
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

        public void InsertAllSystemPermissions()
        {
            var systemGlobalPermissions = Enum.GetNames<GlobalPermissions>();
            var dbGlobalPermissions = _dbContext.GlobalPermissions.ToList();
            foreach (var systemGlobalPermission in systemGlobalPermissions)
            {
                bool isExists = false;
                if (dbGlobalPermissions.Count != 0) 
                {
                    foreach (var dbPermission in dbGlobalPermissions)
                    {
                        if (systemGlobalPermission == dbPermission.Name)
                        {
                            isExists = true;
                            break;
                        }
                    }
                    if (isExists is false)
                    {
                        var permission = new GlobalPermission()
                        {
                            Name = $"{systemGlobalPermission}",
                            Description = $"for {systemGlobalPermission}"
                        };
                        _dbContext.GlobalPermissions.Add(permission);
                        _dbContext.SaveChanges();
                    }
                }
                else
                {
                    var permission = new GlobalPermission()
                    {
                        Name = $"{systemGlobalPermission}",
                        Description = $"for {systemGlobalPermission}"
                    };
                    _dbContext.GlobalPermissions.Add(permission);
                    _dbContext.SaveChanges();
                }
            }
        }

        public void InsertAllSystemRoles()
        {
            var systemGlobalRoles = Enum.GetNames<GlobalRoles>();
            var dbGlobalRoles = _dbContext.GlobalRoles.ToList();
            foreach (var systemGlobalRole in systemGlobalRoles)
            {
                bool isExists = false;
                if (dbGlobalRoles.Count != 0)
                {
                    foreach (var dbGlobalRole in dbGlobalRoles)
                    {
                        if (systemGlobalRole == dbGlobalRole.Name)
                        {
                            isExists = true;
                            break;
                        }
                    }
                    if (isExists is false)
                    {
                        var globalRole = new GlobalRole()
                        {
                            Name = $"{systemGlobalRole}",
                            Description = $"for {systemGlobalRole}"
                        };
                        _dbContext.GlobalRoles.Add(globalRole);
                        _dbContext.SaveChanges();
                    }
                }
                else
                {
                    var globalRole = new GlobalRole()
                    {
                        Name = $"{systemGlobalRole}",
                        Description = $"for {systemGlobalRole}"
                    };
                    _dbContext.GlobalRoles.Add(globalRole);
                    _dbContext.SaveChanges();
                }
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
