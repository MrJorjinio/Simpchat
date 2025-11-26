using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IGlobalRolePermissionRepository : IBaseRepository<GlobalRolePermission>
    {
        Task<Guid?> GetIdAsync(Guid roleId, Guid permissionId);
    }
}
