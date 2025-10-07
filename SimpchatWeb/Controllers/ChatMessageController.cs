using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Entity;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpchatWeb.Controllers
{
    [Route("api/chats/{chatId:guid?}/messages")]
    [ApiController]
    public class ChatMessageController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SimpchatDbContext _dbContext;
        private readonly IChatService _chatService;
        public ChatMessageController(
            ITokenService tokenService,
            IMapper mapper,
            SimpchatDbContext dbContext,
            IChatService chatService
            )
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _dbContext = dbContext;
            _chatService = chatService;
        }

        [HttpPost]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> SendMessageAsync(Guid? chatId, [FromBody] ChatMessagePostDto model)
        {
            var user = HttpContext.Items["RequestData/User"] as User;
            var response = await _chatService.SendMessageAsync(user, chatId, model);
            return response;
        }

    }
}