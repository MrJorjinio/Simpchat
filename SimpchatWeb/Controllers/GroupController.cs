using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Responses;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Token;
using System.Collections.ObjectModel;

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

        [HttpPost]
        [EnsureEntityExistsFilter(typeof(User))]
        public IActionResult CreateGroup(GroupPostDto request)
        {
            var userId = _tokenService.GetUserId(User);      

            var chat = new Chat { };
            chat.Type = ChatType.Group;
            chat.PrivacyType = ChatPrivacyType.Public;

            _dbContext.Chats.Add(chat);
            _dbContext.SaveChanges();
    
            _dbContext.ChatsParticipants.Add(new ChatParticipant { UserId = userId, ChatId = chat.Id });

            var _dbChatPermissions = _dbContext.ChatPermissions;
            var permissionsToInsert = new Collection<ChatUserPermission>();
            foreach (var dbChatPermission in _dbChatPermissions)
            {
                permissionsToInsert.Add(new ChatUserPermission { ChatId = chat.Id, UserId = userId, PermissionId = dbChatPermission.Id });
            }

            _dbContext.ChatsUsersPermissions.AddRange(permissionsToInsert);
            _dbContext.SaveChanges();

            var group = _mapper.Map<Group>(request);
            group.Id = chat.Id;
            group.CreatedById = userId;

            _dbContext.Groups.Add(group);
            _dbContext.SaveChanges();

            var response = _mapper.Map<GroupResponseDto>(group);
            return Ok(response);
        }
    }
}
