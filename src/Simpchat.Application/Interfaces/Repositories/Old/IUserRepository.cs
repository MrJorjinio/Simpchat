using Simpchat.Application.Models.Chats.Search;
using Simpchat.Application.Models.Users.GetById;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories.Old
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<GetByIdUserDto?> GetByIdAsync(Guid id, Guid currentUserId);
        Task<User> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<ICollection<SearchChatResponseDto>?> SearchByUsernameAsync(string searchTerm, Guid currentUserId);
        Task AssignRoleAsync(Guid userId, Guid roleId);
    }
}
