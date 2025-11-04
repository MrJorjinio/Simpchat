using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Simpchat.Shared.Config;
using Simpchat.Web.Middlewares;
using System.Text;

namespace Simpchat.Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddJwtAuthentication(config)
                .AddExceptionHandling()
                .AddSwagger();

            return services;
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var appSettings = config.GetSection("AppSettings").Get<AppSettings>();

            if (appSettings == null)
                throw new Exception("AppSettings section is missing from configuration.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = appSettings.JwtSettings.Issuer,
                        ValidAudience = appSettings.JwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtSettings.Key))
                    };
                });

            return services;
        }

        private static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpchatWeb", Version = "v1" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                     {
                         { securitySchema, new[] { "Bearer" } }
                     });
            });

            return services;
        }

        private static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddProblemDetails(opt =>
            {
                opt.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                };
            });

            return services;
        }
    }
}
