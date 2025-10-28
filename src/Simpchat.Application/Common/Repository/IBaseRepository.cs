using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<Guid> CreateAsync(T entity);
        public Task<List<T>?> GetAllAsync();
        public Task<T?> GetByIdAsync(Guid id);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
    }
}
