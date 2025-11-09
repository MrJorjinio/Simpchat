using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Interfaces
{
    public interface IDataSeeder
    {
        Task SeedAsync();
    }
}
