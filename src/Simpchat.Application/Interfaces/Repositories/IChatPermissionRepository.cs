using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IChatPermissionRepository : IBaseRepository<ChatPermission>
    {
        Task<ChatPermission> GetByNameAsync(string name);
    }
}
