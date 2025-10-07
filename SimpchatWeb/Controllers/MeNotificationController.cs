using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserNotificationDtos.Responses;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpchatWeb.Controllers
{
    [Route("api/me/notifications")]
    [ApiController]
    public class MeNotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public MeNotificationController(
            INotificationService notificationService
            )
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> GetMyNotificationsAsync()
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var response = await _notificationService.GetMyNotificationsAsync(user);
            return response;
        }

        [HttpPatch("mark-as-seen")]
        [EnsureEntityExistsFilter(typeof(User))]
        [EnsureEntityExistsFilter(typeof(Notification))]
        public async Task<IActionResult> MarkAsSeenAsync(Guid messageId)
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var response = await _notificationService.MarkAsSeenAsync(user, messageId);
            return response;
        }

        [HttpDelete]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> DeleteNotificationAsync(Guid messageId)
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var response = await _notificationService.DeleteNotificationAsync(user, messageId);
            return response;
        }
    }
}
