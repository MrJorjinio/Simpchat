using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Interfaces.Token;

namespace SimpchatWeb.Services.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnsureChatPermissionExistsFilterAttribute : Attribute, IActionFilter
    {
        private readonly ChatPermissionType _chatPermissionType;
        private readonly string _idParameterName;

        public EnsureChatPermissionExistsFilterAttribute(ChatPermissionType chatPermissionType, string idParameterName = "id")
        {
            _chatPermissionType = chatPermissionType;
            _idParameterName = idParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<SimpchatDbContext>();
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

            if (!context.ActionArguments.TryGetValue(_idParameterName, out var idObj))
            {
                context.Result = new BadRequestObjectResult($"Parameter '{_idParameterName}' is missing.");
                return;
            }

            if (idObj == null || !Guid.TryParse(idObj.ToString(), out var id))
            {
                context.Result = new BadRequestObjectResult($"Parameter '{_idParameterName}' is invalid.");
                return;
            }

            var chat = db.Chats.Find(id);

            if (chat is null)
            {
                context.Result = new BadRequestObjectResult($"ID '{_idParameterName}' not found.");
                return;
            }

            var userId = tokenService.GetUserId(context.HttpContext.User);

            if (userId == Guid.Empty)
            {
                context.Result = new BadRequestObjectResult($"User ID '{userId}' not found.");
                return;
            }

            var user = db.Users.Find(userId);

            if (user is null)
            {
                context.Result = new BadRequestObjectResult($"User with ID '{userId}' not found.");
                return;
            }

            var chatPermissionTypes = db.ChatsUsersPermissions
                .Where(cup => cup.ChatId == id && cup.UserId == user.Id)
                .Include(sup => sup.Permission)
                .Select(sup => sup.Permission.Name)
                .ToList();

            bool isUserExistsPermission = false;
            foreach (var chatPermissionType in chatPermissionTypes)
            {
                if (_chatPermissionType.ToString() == chatPermissionType)
                {
                    isUserExistsPermission = true;
                    return;
                }
            }

            context.Result = new BadRequestObjectResult($"User '{user.Id} does not have Permission {_chatPermissionType.ToString()}.");
        }
    }
}
