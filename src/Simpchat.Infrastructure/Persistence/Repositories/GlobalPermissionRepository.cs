using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class GlobalPermissionRepository : IGlobalPermissionRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public GlobalPermissionRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Guid> CreateAsync(GlobalPermission entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(GlobalPermission entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<GlobalPermission>?> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalPermission?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalPermission?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(GlobalPermission entity)
        {
            throw new NotImplementedException();
        }
    }
}
