using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Simpchat.Application.Features;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Chats.Post;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Application.Models.Users.Post;
using Simpchat.Application.Models.Users.Update;
using Simpchat.Application.Validators;

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
            services.AddScoped<IMessageService, MessageService>();
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
            services.AddTransient<IValidator<UpdateUserDto>, UpdateUserInfoValidator>();

            return services;
        }
    }
}
