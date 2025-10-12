using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Repositories
{
    public interface IGlobalRoleRepository
    {
        Task CreateAsync(GlobalRole role);
        Task UpdateAsync(GlobalRole role);
        Task DeleteAsync(GlobalRole role);
        Task<GlobalRole?> GetByIdAsync(Guid id);
        Task<GlobalRole?> GetByNameAsync(string name);
        Task AssignPermission(Guid roleId, Guid permissionId);
    }
}
