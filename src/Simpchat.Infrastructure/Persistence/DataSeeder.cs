using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Auth;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;
using Simpchat.Infrastructure.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence
{
    public class DataSeeder : IDataSeeder
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IGlobalRoleRepository _globalRoleRepo;
        private readonly IUserRepository _userRepo;

        public DataSeeder(
            SimpchatDbContext dbContext,
            IPasswordHasher passwordHasher,
            IGlobalRoleRepository globalRoleRepo,
            IUserRepository userRepo)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _globalRoleRepo = globalRoleRepo;
            _userRepo = userRepo;
        }

        public async Task SeedAsync()
        {
            await SyncGlobalPermissionsAsync();
            await SyncGlobalRolesAsync();
            await SetDefaultAdminAsync();
        }

        private async Task SetDefaultAdminAsync()
        {
            if (await _userRepo.GetByUsernameAsync("admin") is null)
            {
                var salt = Guid.NewGuid().ToString();
                var defaultRole = await _globalRoleRepo.GetByNameAsync(Enum.GetName(GlobalRoleType.Admin));

                if (defaultRole is null)
                {
                    throw new InvalidOperationException("Admin role doesn't exists");
                }

                var admin = new User
                {
                    Username = "admin",
                    Salt = salt,
                    PasswordHash = await _passwordHasher.EncryptAsync("admin", salt),
                    Description = "Admin of Simpchat",
                    Email = "dtrum4933@gmail.com",
                    HwoCanAddType = HwoCanAddYouTypes.Nobody,
                    RoleId = defaultRole.Id
                };

                await _userRepo.CreateAsync(admin);
            }
        }

        private async Task SyncGlobalPermissionsAsync()
        {
            var dbPermissions = await _dbContext.GlobalPermissions
                .Select(gp => gp.Name)
                .ToListAsync();
            var systemPermissions = Enum.GetValues<GlobalPermissionType>()
                .Select(gpt => Enum.GetName(gpt))
                .ToList();

            var notAddedPermissions = new List<GlobalPermission>();

            foreach (var systemPermission in systemPermissions)
            {
                if (dbPermissions.Contains(systemPermission) is false)
                {
                    var globalPermission = new GlobalPermission
                    {
                        Name = systemPermission,
                        Description = $"for {systemPermission}"
                    };

                    notAddedPermissions.Add(globalPermission);
                }
            }

            await _dbContext.GlobalPermissions.AddRangeAsync(notAddedPermissions);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SyncGlobalRolesAsync()
        {
            var dbRoles = await _dbContext.GlobalRoles
                .Select(gr => gr.Name)
                .ToListAsync();
            var systemRoles = Enum.GetValues<GlobalRoleType>()
                .Select(grp => Enum.GetName(grp))
                .ToList();

            var notAddedRoles = new List<GlobalRole>();

            foreach (var systemRole in systemRoles)
            {
                if (dbRoles.Contains(systemRole) is false)
                {
                    var dbRole = new GlobalRole
                    {
                        Name = systemRole,
                        Description = $"for {systemRole}"
                    };

                    notAddedRoles.Add(dbRole);
                }
            }

            await _dbContext.GlobalRoles.AddRangeAsync(notAddedRoles);
            await _dbContext.SaveChangesAsync();
        }
    }
}
