using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Interfaces.Token;

namespace SimpchatWeb.Services.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnsureEntityExistsFilterAttribute : Attribute, IActionFilter
    {
        private readonly Type _entityType;
        private readonly string _idParameterName;

        public EnsureEntityExistsFilterAttribute(Type entityType, string idParameterName = "id")
        {
            _entityType = entityType;
            _idParameterName = idParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }

        public void OnActionExecuting(ActionExecutingContext context)
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
                Type t when t == typeof(Chat) => db.Chats.Find(id),
                Type t when t == typeof(User) => db.Users.Find(id),
                Type t when t == typeof(Message) => db.Messages.Find(id),
                Type t when t == typeof(ChatParticipant) => db.ChatsParticipants.FirstOrDefault(cp => cp.UserId == userId && cp.ChatId == id),
                Type t when t == typeof(Notification) => db.Notifications.FirstOrDefault(n => n.MessageId == id),
                _ => null
            };

            if (entity is null)
            {
                context.Result = new BadRequestObjectResult($"{_entityType.Name} with ID '{id}' not found.");
            }
        }

    }
}
