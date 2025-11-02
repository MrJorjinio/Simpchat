using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IChatRepository : IBaseRepository<Chat>
    {
        Task AddUserPermissionAsync(ChatUserPermission chatUserPermission);
    }
}
