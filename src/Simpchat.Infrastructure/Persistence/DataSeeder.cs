using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
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
            await SyncChatPermissionsAsync();
            await SetDefaultAdminAsync();
        }

        private async Task SetDefaultAdminAsync()
        {
            if (await _userRepo.GetByUsernameAsync("admin") is null)
            {
                var salt = Guid.NewGuid().ToString();
                var defaultRole = await _globalRoleRepo.GetByNameAsync(Enum.GetName(GlobalRoleTypes.Admin));

                if (defaultRole is null)
                {
                    throw new InvalidOperationException("Admin role doesn't exists");
                }

                var admin = new User
                {
                    Username = "admin",
                    Salt = salt,
                    PasswordHash = await _passwordHasher.EncryptAsync("1234567890", salt),
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
            var dbPermissions = await _dbContext.GlobalPermissions.ToListAsync();
            var dbNames = dbPermissions.Select(gp => gp.Name).ToHashSet();

            var systemPermissions = Enum.GetValues<GlobalPermissionTypes>()
                .Select(gpt => gpt.GetDisplayName())
                .ToHashSet();

            var notAddedPermissions = systemPermissions
                .Except(dbNames)
                .Select(name => new GlobalPermission
                {
                    Name = name,
                    Description = $"for {name}"
                })
                .ToList();

            var notExistingPermissions = dbPermissions
                .Where(db => !systemPermissions.Contains(db.Name))
                .ToList();

            if (notExistingPermissions.Any())
                _dbContext.GlobalPermissions.RemoveRange(notExistingPermissions);

            if (notAddedPermissions.Any())
                await _dbContext.GlobalPermissions.AddRangeAsync(notAddedPermissions);

            await _dbContext.SaveChangesAsync();
        }

        private async Task SyncGlobalRolesAsync()
        {
            var dbRoles = await _dbContext.GlobalRoles.ToListAsync();
            var dbNames = dbRoles.Select(gr => gr.Name).ToHashSet();

            var systemRoles = Enum.GetValues<GlobalRoleTypes>()
                .Select(grp => grp.GetDisplayName())
                .ToHashSet();

            var notAddedRoles = systemRoles
                .Except(dbNames)
                .Select(name => new GlobalRole
                {
                    Name = name,
                    Description = $"for {name}"
                })
                .ToList();

            var notExistingRoles = dbRoles
                .Where(db => !systemRoles.Contains(db.Name))
                .ToList();

            if (notExistingRoles.Any())
                _dbContext.GlobalRoles.RemoveRange(notExistingRoles);

            if (notAddedRoles.Any())
                await _dbContext.GlobalRoles.AddRangeAsync(notAddedRoles);

            await _dbContext.SaveChangesAsync();
        }

        private async Task SyncChatPermissionsAsync()
        {
            var dbPermissions = await _dbContext.ChatPermissions.ToListAsync();
            var dbNames = dbPermissions.Select(cp => cp.Name).ToHashSet();

            var systemPermissions = Enum.GetValues<ChatPermissionTypes>()
                .Select(cp => cp.GetDisplayName())
                .ToHashSet();

            var notAddedPermissions = systemPermissions
                .Except(dbNames)
                .Select(name => new ChatPermission
                {
                    Name = name
                })
                .ToList();

            var notExistingPermissions = dbPermissions
                .Where(db => !systemPermissions.Contains(db.Name))
                .ToList();

            if (notExistingPermissions.Any())
                _dbContext.ChatPermissions.RemoveRange(notExistingPermissions);

            if (notAddedPermissions.Any())
                await _dbContext.ChatPermissions.AddRangeAsync(notAddedPermissions);

            await _dbContext.SaveChangesAsync();
        }
    }
}
