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
    public class ChatPermissionRepository : IChatPermissionRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public ChatPermissionRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(ChatPermission entity)
        {
            _dbContext.ChatPermissions.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(ChatPermission entity)
        {
            _dbContext.ChatPermissions.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ChatPermission>?> GetAllAsync()
        {
            return await _dbContext.ChatPermissions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ChatPermission?> GetByIdAsync(Guid id)
        {
            return await _dbContext.ChatPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ChatPermission> GetByNameAsync(string name)
        {
            var res = await _dbContext.ChatPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name);
            return res!;
        }

        public async Task UpdateAsync(ChatPermission entity)
        {
            _dbContext.ChatPermissions.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
