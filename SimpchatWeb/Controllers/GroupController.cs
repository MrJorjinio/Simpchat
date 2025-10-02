using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Creates;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Gets;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Responses;
using SimpchatWeb.Services.Interfaces.Token;

namespace SimpchatWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SimpchatDbContext _dbContext;
        public GroupController(
            ITokenService tokenService, 
            IMapper mapper, 
            SimpchatDbContext dbContext
            )
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Get()
        {
            var response = _mapper.Map<ICollection<GroupGetDto>>(_dbContext.Groups);
            return Ok(response);
        }

        [HttpGet("search-groups/{name}")]
        public IActionResult SearchGroups(
            string name
            )
        {
            var groups = _dbContext.Groups.Where(g => g.Name == name);
            var response = _mapper.Map<ICollection<GroupResponseDto>>(groups);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(
            Guid id
            )
        {
            var group = _dbContext.Groups.Find(id);

            if (group is null)
            {
                return BadRequest();
            }

            var response = _mapper.Map<GroupResponseDto>(group);

            return Ok(response);
        }

        [HttpPost("me")]
        public IActionResult CreateMyGroup(
            GroupPostDto request
            )
        {
            var userId = _tokenService.GetUserId(User);
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var chat = new Chat()
            {
                Id = Guid.NewGuid(),
                Type = Services.Db.Contexts.Default.Enums.ChatTypes.Group
            };

            _dbContext.Add(chat);
            _dbContext.SaveChanges();

            var group = _mapper.Map<Group>(request);
            group.Id = chat.Id;
            group.CreatedById = userId;

            _dbContext.Add(group);
            _dbContext.SaveChanges();

            var response = _mapper.Map<GroupResponseDto>(group);

            var user = _dbContext.Users.Find(userId);
            response.CreatedBy = user.Username;

            return Ok(response);
        }
    }
}
