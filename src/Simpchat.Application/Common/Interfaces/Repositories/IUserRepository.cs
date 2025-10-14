using Simpchat.Application.Common.Models.Chats.Search;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<ICollection<ChatSearchResponseDto>?> SearchByUsernameAsync(string searchTerm, Guid currentUserId);
        Task AssignRoleAsync(Guid userId, Guid roleId);
    }
}
