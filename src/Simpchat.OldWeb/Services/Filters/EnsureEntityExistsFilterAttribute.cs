using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Interfaces.Auth;

namespace SimpchatWeb.Services.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnsureEntityExistsFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly Type _entityType;
        private readonly string _idParameterName;

        public EnsureEntityExistsFilterAttribute(Type entityType, string idParameterName = "")
        {
            _entityType = entityType;

            if (string.IsNullOrEmpty(idParameterName))
            {
                if (_entityType == typeof(Notification))
                    _idParameterName = "messageId";
                else if (_entityType == typeof(ChatParticipant))
                    _idParameterName = "chatId";
                else
                    _idParameterName = $"{_entityType.Name.ToLower()}Id";
            }
            else
            {
                _idParameterName = idParameterName;
            }
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<SimpchatDbContext>();
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

            context.ActionArguments.TryGetValue(_idParameterName, out var idObj);

            var userId = tokenService.GetUserId(context.HttpContext.User);
            bool isUserByToken = userId != Guid.Empty;

            Guid id = Guid.Empty;

            if (idObj != null && Guid.TryParse(idObj.ToString(), out var parsedId))
                id = parsedId;
            else if (_entityType == typeof(User) && isUserByToken)
                id = userId;
            else if (_entityType != typeof(User))
            {
                context.Result = new BadRequestObjectResult($"Parameter '{_idParameterName}' is missing or invalid.");
                return;
            }

            object entity = _entityType switch
            {
                Type t when t == typeof(Chat) => await db.Chats.FindAsync(id),
                Type t when t == typeof(User) => await db.Users.FindAsync(id),
                Type t when t == typeof(Message) => await db.Messages.FindAsync(id),
                Type t when t == typeof(ChatParticipant) =>
                    await db.ChatsParticipants.FirstOrDefaultAsync(cp => cp.UserId == userId && cp.ChatId == id),
                Type t when t == typeof(Notification) =>
                    await db.Notifications.Where(n => n.MessageId == id).ToListAsync(),
                _ => null
            };

            if (entity is null)
            {
                context.Result = new BadRequestObjectResult($"{_entityType.Name} with ID '{id}' not found.");
                return;
            }

            context.HttpContext.Items[$"RequestData/{_entityType.Name}"] = entity;
            await next();
        }
    }
}
