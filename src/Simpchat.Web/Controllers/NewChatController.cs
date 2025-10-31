using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Interfaces.Services.New;

namespace Simpchat.Web.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class NewChatController : ControllerBase
    {
        private readonly INewChatService _chatService;
        public NewChatController(INewChatService chatService)
        {
            _chatService = chatService;
        }

    }
}
