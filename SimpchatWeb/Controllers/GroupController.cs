using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Responses;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Auth;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpchatWeb.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public GroupController(
            SimpchatDbContext dbContext,
            IMapper mapper,
            ITokenService tokenService
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPatch]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> CreateGroupAsync(GroupPostDto request)
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var chat = new Chat
            {
                Type = ChatType.Group,
                PrivacyType = ChatPrivacyType.Public
            };

            await _dbContext.Chats.AddAsync(chat);
            await _dbContext.SaveChangesAsync();

            await _dbContext.ChatsParticipants.AddAsync(new ChatParticipant { UserId = user.Id, ChatId = chat.Id });

            var _dbChatPermissions = _dbContext.ChatPermissions;
            var permissionsToInsert = new Collection<ChatUserPermission>();
            foreach (var dbChatPermission in _dbChatPermissions)
            {
                permissionsToInsert.Add(new ChatUserPermission { ChatId = chat.Id, UserId = user.Id, PermissionId = dbChatPermission.Id });
            }

            await _dbContext.ChatsUsersPermissions.AddRangeAsync(permissionsToInsert);
            await _dbContext.SaveChangesAsync();

            var group = _mapper.Map<Group>(request);
            group.Id = chat.Id;
            group.CreatedById = user.Id;

            await _dbContext.Groups.AddAsync(group);
            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<GroupResponseDto>(group);
            return Ok(response);
        }
    }
}
