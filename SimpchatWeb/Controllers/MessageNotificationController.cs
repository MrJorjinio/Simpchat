using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Token;

namespace SimpchatWeb.Controllers
{
    [Route("api/messages/{messageId}/notifications")]
    [ApiController]
    public class MessageNotificationController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SimpchatDbContext _dbContext;

        public MessageNotificationController(
            ITokenService tokenService,
            IMapper mapper,
            SimpchatDbContext dbContext
            )
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpPatch("me")]
        [EnsureEntityExistsFilter(typeof(User))]
        [EnsureEntityExistsFilter(typeof(Notification), "messageId")]
        public IActionResult MarkAsSeen(
            Guid messageId
            )
        {
            var userId = _tokenService.GetUserId(User);
            var dbUser = _dbContext.Users
                .Find(userId);

            var notification = _dbContext.Notifications
                .FirstOrDefault(n => n.MessageId == messageId && n.IsSeen == false);

            if (notification is null)
            {
                return BadRequest();
            }

            notification.IsSeen = true;
            _dbContext.SaveChanges();

            var response = _mapper.Map<UserNotificationMarkAsSeenPutResponseDto>(notification);

            return Ok();
        }

        [HttpDelete]
        [EnsureEntityExistsFilter(typeof(User))]
        public IActionResult DeleteNotification(Guid messageId)
        {
            var userId = _tokenService.GetUserId(User);

            var notification = _dbContext.Notifications
                .FirstOrDefault(n => n.UserId == userId && n.MessageId == messageId);

            if (notification is null)
            {
                return BadRequest();
            }

            _dbContext.Notifications.Remove(notification);
            _dbContext.SaveChanges();

            return Ok();
        }
    }
}
