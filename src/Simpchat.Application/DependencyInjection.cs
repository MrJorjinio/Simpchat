using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simpchat.Application.Features.Old.Channels;
using Simpchat.Application.Features.Old.Chats;
using Simpchat.Application.Features.Old.Conversations;
using Simpchat.Application.Features.Old.Groups;
using Simpchat.Application.Features.Old.Notifications;
using Simpchat.Application.Features.Old.Users;
using Simpchat.Application.Interfaces.Auth;
using Simpchat.Application.Interfaces.Services.Old;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Application.Models.Users.Post;
using Simpchat.Application.Models.Users.Update;
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
            services.AddTransient<IValidator<PostMessageApiRequestDto>, PostMessageApiRequestValidator>();
            services.AddTransient<IValidator<PutChatDto>,PutChatValidator >();
            services.AddTransient<IValidator<UpdateUserInfoDto>, UpdateUserInfoValidator>();

            return services;
        }
    }
}
