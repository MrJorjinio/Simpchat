using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Extentions;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.Permissions;
using System.Security.Claims;

namespace Simpchat.Web.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost("grant")]
        public async Task<IActionResult> GrantPermissionAsync([FromBody] GrantPermissionDto dto)
        {
            var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _permissionService.GrantPermissionAsync(dto, requesterId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokePermissionAsync([FromBody] RevokePermissionDto dto)
        {
            var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _permissionService.RevokePermissionAsync(dto, requesterId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpGet("{chatId}/user/{userId}")]
        public async Task<IActionResult> GetUserPermissionsAsync(Guid chatId, Guid userId)
        {
            var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _permissionService.GetUserPermissionsAsync(chatId, userId, requesterId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpGet("{chatId}/all")]
        public async Task<IActionResult> GetAllChatPermissionsAsync(Guid chatId)
        {
            var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _permissionService.GetAllChatPermissionsAsync(chatId, requesterId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }

        [HttpDelete("{chatId}/user/{userId}")]
        public async Task<IActionResult> RevokeAllPermissionsAsync(Guid chatId, Guid userId)
        {
            var requesterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var response = await _permissionService.RevokeAllPermissionsAsync(chatId, userId, requesterId);
            var apiResponse = response.ToApiResult();

            return apiResponse.ToActionResult();
        }
    }
}
