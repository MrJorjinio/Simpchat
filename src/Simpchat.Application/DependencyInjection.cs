using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simpchat.Application.Common.Interfaces.Auth;
using Simpchat.Application.Common.Interfaces.Services;
using Simpchat.Application.Features.Chats;
using Simpchat.Application.Features.Users;
using Simpchat.Application.Features.Users.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddServices();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChatService, ChatService>();

            return services;
        }
    }
}
