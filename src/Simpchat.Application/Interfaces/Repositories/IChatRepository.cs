using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IChatRepository : IBaseRepository<Chat>
    {
        Task AddUserPermissionAsync(ChatUserPermission chatUserPermission);
    }
}
