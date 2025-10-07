using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnsureChatTypeNotFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ChatType _chatType;
        private readonly string _idParameterName;

        public EnsureChatTypeNotFilterAttribute(ChatType chatType, string idParameterName = "chatId")
        {
            _chatType = chatType;
            _idParameterName = idParameterName;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<SimpchatDbContext>();

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

            var chat = await db.Chats.FindAsync(id);

            if (chat is null)
            {
                context.Result = new BadRequestObjectResult($"Chat with ID '{id}' not found.");
                return;
            }

            if (chat.Type == _chatType)
            {
                context.Result = new BadRequestObjectResult($"Chat type '{_chatType}' is not allowed.");
                return;
            }

            context.HttpContext.Items["RequestData/Chat"] = chat;
            await next();
        }
    }
}
