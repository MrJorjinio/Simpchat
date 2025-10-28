using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Interfaces.Repositories.Old;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Users.GetById;
using Simpchat.Domain.Entities;
using Simpchat.Infrastructure.Identity;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;

namespace Simpchat.Infrastructure.Persistence.Repositories.Old
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

        public async Task<GetByIdUserDto?> GetByIdAsync(Guid id, Guid currentUserId)
        {
            var dto = _dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => new GetByIdUserDto
                {
                    Description = u.Description,
                    AvatarUrl = u.AvatarUrl,
                    ChatId = _dbContext.Conversations.FirstOrDefault(c => (c.UserId1 == currentUserId && c.UserId2 == id) || (c.UserId1 == id && c.UserId2 == currentUserId)).Id,
                    IsOnline = u.LastSeen.AddMinutes(5) > DateTimeOffset.UtcNow,
                    LastSeen = u.LastSeen,
                    UserId = u.Id,
                    Username = u.Username
                })
                .FirstOrDefault();

            return dto;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return _dbContext.Users
                .Include(u => u.GlobalRoles)
                    .ThenInclude(g => g.Role)
                .FirstOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users
                .Include(u => u.GlobalRoles)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }


        public async Task<ICollection<SearchChatResponseDto>?> SearchByUsernameAsync(string searchTerm, Guid currentUserId)
        {
            var users = _dbContext.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{searchTerm}%"));

            var usersDtos = await users.Select(u => new SearchChatResponseDto
            {
                ChatId = _dbContext.Conversations.FirstOrDefault(c => (c.UserId1 == currentUserId && c.UserId2 == u.Id) || (c.UserId1 == u.Id && c.UserId2 == currentUserId)).Id,
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
