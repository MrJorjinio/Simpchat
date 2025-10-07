using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Interfaces.Auth;

namespace SimpchatWeb.Services.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnsureChatPermissionExistsFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ChatPermissionType _chatPermissionType;
        private readonly string _idParameterName;

        public EnsureChatPermissionExistsFilterAttribute(ChatPermissionType chatPermissionType, string idParameterName = "chatId")
        {
            _chatPermissionType = chatPermissionType;
            _idParameterName = idParameterName;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<SimpchatDbContext>();
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

            if (!context.ActionArguments.TryGetValue(_idParameterName, out var idObj)
                || idObj == null
                || !Guid.TryParse(idObj.ToString(), out var chatId))
            {
                context.Result = new BadRequestObjectResult($"Parameter '{_idParameterName}' is missing or invalid.");
                return;
            }

            var chat = await db.Chats.FindAsync(chatId);
            if (chat is null)
            {
                context.Result = new BadRequestObjectResult($"Chat with ID '{chatId}' not found.");
                return;
            }

            var userId = tokenService.GetUserId(context.HttpContext.User);
            if (userId == Guid.Empty)
            {
                context.Result = new BadRequestObjectResult("User ID not found in token.");
                return;
            }

            var user = await db.Users.FindAsync(userId);
            if (user is null)
            {
                context.Result = new BadRequestObjectResult($"User with ID '{userId}' not found.");
                return;
            }

            var userPermissions = await db.ChatsUsersPermissions
                .Where(cup => cup.ChatId == chatId && cup.UserId == userId)
                .Include(cup => cup.Permission)
                .Select(cup => cup.Permission.Name)
                .ToListAsync();

            if (!userPermissions.Contains(_chatPermissionType.ToString()))
            {
                context.Result = new BadRequestObjectResult($"User '{user.Id}' does not have permission '{_chatPermissionType}'.");
                return;
            }

            context.HttpContext.Items[$"RequestData/User"] = user;
            context.HttpContext.Items[$"RequestData/Chat"] = chat;
            await next();
        }
    }
}
