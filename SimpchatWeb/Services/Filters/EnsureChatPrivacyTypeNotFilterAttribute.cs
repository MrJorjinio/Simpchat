using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Interfaces.Token;

namespace SimpchatWeb.Services.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class EnsureChatPrivacyTypeNotFilterAttribute : Attribute, IActionFilter
    {
        private readonly ChatPrivacyType _chatPrivacyType;
        private readonly string _idParameterName;

        public EnsureChatPrivacyTypeNotFilterAttribute(ChatPrivacyType chatPrivacyType, string idParameterName = "id")
        {
            _chatPrivacyType = chatPrivacyType;
            _idParameterName = idParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {}

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

            if (chat.PrivacyType == _chatPrivacyType)
            {
                context.Result = new BadRequestObjectResult($"Chat type '{_chatPrivacyType}' is not valid.");
            }
        }
    }
}
