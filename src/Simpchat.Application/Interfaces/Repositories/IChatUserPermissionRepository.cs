using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IChatUserPermissionRepository : IBaseRepository<ChatUserPermission>
    {
        Task<ChatUserPermission?> GetByUserChatPermissionAsync(Guid chatId, Guid userId, Guid permissionId);
        Task<List<ChatUserPermission>> GetUserChatPermissionsAsync(Guid chatId, Guid userId);
        Task<List<ChatUserPermission>> GetChatPermissionsAsync(Guid chatId);
        Task DeleteByUserChatPermissionAsync(Guid chatId, Guid userId, Guid permissionId);
    }
}
