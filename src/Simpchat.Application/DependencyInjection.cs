using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simpchat.Application.Features.Channels;
using Simpchat.Application.Features.Chats;
using Simpchat.Application.Features.Conversations;
using Simpchat.Application.Features.Groups;
using Simpchat.Application.Features.Notifications;
using Simpchat.Application.Features.Users;
using Simpchat.Application.Features.Users.Services;
using Simpchat.Application.Interfaces.Auth;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Application.Models.Users.Post;
using Simpchat.Application.Validators;
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
            services
                .AddServices()
                .AddValidation();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IChatMessageService, ChatMessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IConversationService, ConversationService>();

            return services;
        }

        private static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddTransient<IValidator<RegisterUserDto>, RegisterUserValidator>();
            services.AddTransient<IValidator<PostChatDto>, PostChatValidator>();
            services.AddTransient<IValidator<PostMessageApiRequestDto>, PostMessageValidator>();

            return services;
        }
    }
}
