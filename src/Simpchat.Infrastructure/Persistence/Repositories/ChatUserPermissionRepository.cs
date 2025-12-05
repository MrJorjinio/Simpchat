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
    public class ChatUserPermissionRepository : IChatUserPermissionRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public ChatUserPermissionRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(ChatUserPermission entity)
        {
            _dbContext.ChatsUsersPermissions.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(ChatUserPermission entity)
        {
            _dbContext.ChatsUsersPermissions.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ChatUserPermission>?> GetAllAsync()
        {
            return await _dbContext.ChatsUsersPermissions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ChatUserPermission?> GetByIdAsync(Guid id)
        {
            return await _dbContext.ChatsUsersPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(ChatUserPermission entity)
        {
            _dbContext.ChatsUsersPermissions.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ChatUserPermission?> GetByUserChatPermissionAsync(Guid chatId, Guid userId, Guid permissionId)
        {
            return await _dbContext.ChatsUsersPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ChatId == chatId && p.UserId == userId && p.PermissionId == permissionId);
        }

        public async Task<List<ChatUserPermission>> GetUserChatPermissionsAsync(Guid chatId, Guid userId)
        {
            return await _dbContext.ChatsUsersPermissions
                .AsNoTracking()
                .Include(p => p.Permission)
                .Include(p => p.User)
                .Where(p => p.ChatId == chatId && p.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<ChatUserPermission>> GetChatPermissionsAsync(Guid chatId)
        {
            return await _dbContext.ChatsUsersPermissions
                .AsNoTracking()
                .Include(p => p.Permission)
                .Include(p => p.User)
                .Where(p => p.ChatId == chatId)
                .ToListAsync();
        }

        public async Task DeleteByUserChatPermissionAsync(Guid chatId, Guid userId, Guid permissionId)
        {
            var permission = await GetByUserChatPermissionAsync(chatId, userId, permissionId);
            if (permission is not null)
            {
                _dbContext.ChatsUsersPermissions.Remove(permission);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
