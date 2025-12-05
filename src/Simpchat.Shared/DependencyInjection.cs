using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simpchat.Shared.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddSingleton(jwtSettings);

            var connectionStrings = config.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            services.AddSingleton(connectionStrings);

            var minioSettings = config.GetSection("MinioSettings").Get<MinioSettings>();
           services.AddSingleton(minioSettings);

            var rabbitMQSettings = config.GetSection("RabbitMQ").Get<RabbitMQSettings>();
            services.AddSingleton(rabbitMQSettings);

            var emailSettings = config.GetSection("EmailConfiguration").Get<EmailSettings>();
            services.AddSingleton(emailSettings);
            return services;
        }
    }
}
