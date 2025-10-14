using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Domain.Entities;
using Simpchat.Infrastructure.Identity;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public UserRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AssignRoleAsync(Guid userId, Guid roleId)
        {
            await _dbContext.UsersGlobalRoles.AddAsync(new GlobalRoleUser { UserId = userId, RoleId = roleId });
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return _dbContext.Users
                .Include(u => u.GlobalRoles)
                .FirstOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users
                .Include(u => u.GlobalRoles)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<ICollection<ChatSearchResponseDto>?> SearchByUsernameAsync(string searchTerm, Guid currentUserId)
        {
            var users = _dbContext.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{searchTerm}%"));

            var chatId = _dbContext.Conversations
                .FirstOrDefault(c => users.Any(u =>
                (u.Id == c.UserId1 && c.UserId2 == currentUserId)
                ||
                (u.Id == currentUserId && c.UserId2 == u.Id)
                ))?.Id;

            var usersDtos = await users.Select(u => new ChatSearchResponseDto
            {
                ChatId = chatId,
                DisplayName = u.Username,
                ChatType = ChatType.Conversation,
                EntityId = u.Id,
                AvatarUrl = u.AvatarUrl
            }).ToListAsync();

            return usersDtos;
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
