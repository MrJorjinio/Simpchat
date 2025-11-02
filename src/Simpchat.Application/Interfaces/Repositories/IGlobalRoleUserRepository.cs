using Simpchat.Application.Common.Repository;
using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IGlobalRoleUserRepository : IBaseRepository<GlobalRoleUser>
    {
        Task<List<GlobalRole>> GetUserRolesAsync(Guid userId);
    }
}
