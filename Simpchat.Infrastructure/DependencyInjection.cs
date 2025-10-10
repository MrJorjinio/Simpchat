using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Simpchat.Infrastructure.Persistence;
using SimpchatWeb.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var appSettings = new AppSettings();
            config.GetSection("AppSettings").Bind(appSettings);

            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            services.AddDbContext<SimpchatDbContext>(options =>
            {
                options.UseNpgsql(appSettings.ConnectionStrings.Default);
            });

            return services;
        }
    }
}
