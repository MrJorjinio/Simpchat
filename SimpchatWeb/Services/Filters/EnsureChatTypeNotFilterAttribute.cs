using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnsureChatTypeNotFilterAttribute : Attribute, IActionFilter
    {
        private readonly ChatType _chatType;
        private readonly string _idParameterName;

        public EnsureChatTypeNotFilterAttribute(ChatType chatType, string idParameterName = "id")
        {
            _chatType = chatType;
            _idParameterName = idParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }

        public void OnActionExecuting(ActionExecutingContext context)
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

            var chat = db.Chats.Find(id);

            if (chat is null)
            {
                context.Result = new BadRequestObjectResult($"ID '{_idParameterName}' not found.");
                return;
            }

            if (chat.Type == _chatType)
            {
                context.Result = new BadRequestObjectResult($"Chat type '{_chatType}' is not valid.");
            }
        }
    }
}
