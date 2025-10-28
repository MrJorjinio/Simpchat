using Simpchat.Application.Common.Repository;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories.New
{
    public interface INewUserRepository : IBaseRepository<User>, ISearchableRepository<User>
    { }
}
