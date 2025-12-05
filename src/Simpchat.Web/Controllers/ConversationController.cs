using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.Services;

using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/conversations")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid conversationId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _conversationService.DeleteAsync(conversationId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }
    }
}
