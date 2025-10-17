using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnsureChatPrivacyTypeNotFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ChatPrivacyType _chatPrivacyType;
        private readonly string _idParameterName;

        public EnsureChatPrivacyTypeNotFilterAttribute(ChatPrivacyType chatPrivacyType, string idParameterName = "chatId")
        {
            _chatPrivacyType = chatPrivacyType;
            _idParameterName = idParameterName;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<SimpchatDbContext>();

            if (!context.ActionArguments.TryGetValue(_idParameterName, out var idObj)
                || idObj == null
                || !Guid.TryParse(idObj.ToString(), out var id))
            {
                context.Result = new BadRequestObjectResult($"Parameter '{_idParameterName}' is missing or invalid.");
                return;
            }

            var chat = await db.Chats.FindAsync(id);
            if (chat is null)
            {
                context.Result = new BadRequestObjectResult($"Chat with ID '{id}' not found.");
                return;
            }

            if (chat.PrivacyType == _chatPrivacyType)
            {
                context.Result = new BadRequestObjectResult($"Chat privacy type '{_chatPrivacyType}' is not allowed.");
                return;
            }

            context.HttpContext.Items["RequestData/Chat"] = chat;
            await next();
        }
    }
}
