using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IGlobalRoleRepository : IBaseRepository<GlobalRole>
    {
        Task<GlobalRole?> GetByNameAsync(string name);
    }
}
