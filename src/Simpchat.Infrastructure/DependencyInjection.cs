using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Simpchat.Application.Common.Interfaces.Auth;
using Simpchat.Application.Common.Interfaces.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Infrastructure.ExternalServices.FileStorage;
using Simpchat.Infrastructure.Persistence;
using Simpchat.Infrastructure.Persistence.Repositories;
using Simpchat.Infrastructure.Security;
using Simpchat.Shared.Config;
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
            services
                .AddPersistence(config)
                .AddSecurity()
                .AddFileStorage(config);

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {
            var appSettings = config.GetSection("AppSettings").Get<AppSettings>();

            if (appSettings == null)
                throw new Exception("AppSettings section is missing from configuration.");

            services.AddDbContext<SimpchatDbContext>(options =>
            {
                options.UseNpgsql(appSettings.ConnectionStrings.Default);
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGlobalRoleRepository, GlobalRoleRepository>();
            services.AddScoped<IGlobalPermissionRepository, GlobalPermissionRepository>();

            return services;
        }

        private static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }

        private static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration config)
        {
            var appSettings = config.GetSection("AppSettings").Get<AppSettings>();

            if (appSettings == null)
                throw new Exception("AppSettings section is missing from configuration.");

            services.AddScoped<IFileStorageService, FileStorageService>();

            services.AddSingleton<IMinioClient>(sp =>
            {
                var minioSettings = appSettings.MinioSettings;
                if (appSettings.MinioSettings == null)
                {
                    throw new InvalidOperationException("MinioSettings is not configured in appsettings.json");
                }

                var client = new MinioClient()
                    .WithEndpoint(minioSettings.Endpoint)
                    .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey);

                if (minioSettings.UseSsl)
                {
                    client = client.WithSSL();
                }

                return client.Build();
            });

            return services;
        }
    }
}
