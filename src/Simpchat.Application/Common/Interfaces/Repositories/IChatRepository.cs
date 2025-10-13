using Simpchat.Application.Common.Models.Chats;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task<Chat?> GetByIdAsync(Guid id);
        Task<ICollection<Chat>?> GetUserChatsAsync(Guid userId);
        Task<ICollection<Chat>?> SearchByNameAsync(string searchTerm, Guid userId);
        Task CreateAsync(Chat chat);
        Task DeleteAsync(Chat chat);
        Task UpdateAsync(Chat chat);
        Task AddMessageAsync(Message message);
    }
}
