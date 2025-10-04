using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Token;

namespace SimpchatWeb.Controllers
{
    [Route("api/me/notifications")]
    [ApiController]
    public class MeNotificationController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SimpchatDbContext _dbContext;
        public MeNotificationController(
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
        [EnsureEntityExistsFilter(typeof(User))]
        public IActionResult GetMyNotifications()
        {
            var userId = _tokenService.GetUserId(User);
            var dbUser = _dbContext.Users
                .Find(userId);

            var userNotifications = _dbContext.Notifications
                .Where(un => un.UserId == userId && un.IsSeen == false)
                .Include(un => un.Message)
                .ToList();

            var response = _mapper.Map<ICollection<UserNotificationsResponseDto>>(userNotifications);
            return Ok(response);
        }
    }
}
