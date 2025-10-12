using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Repositories
{
    public interface IGlobalPermissionRepository
    {
        Task CreateAsync(GlobalPermission permission);
        Task UpdateAsync(GlobalPermission permission);
        Task DeleteAsync(GlobalPermission permission);
        Task<GlobalPermission?> GetByIdAsync(Guid id);
        Task<GlobalPermission?> GetByNameAsync(string name);
    }
}
