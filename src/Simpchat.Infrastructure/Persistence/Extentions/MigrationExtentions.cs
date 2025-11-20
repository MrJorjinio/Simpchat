using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Extentions
{
    public static class MigrationExtentions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using SimpchatDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<SimpchatDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
