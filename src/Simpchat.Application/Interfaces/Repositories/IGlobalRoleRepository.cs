using Simpchat.Application.Common.Repository;
using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IGlobalRoleRepository : IBaseRepository<GlobalRole>
    {
        Task<GlobalRole?> GetByNameAsync(string name);
    }
}
