using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using SimpchatWeb.Services.Filters;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpchatWeb.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(
            IUserService userService
            )
        {
            _userService = userService;
        }

        [HttpGet("{userId:guid}")]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> GetUserByUserIdAsync(Guid userId)
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var response = await _userService.GetUserByIdAsync(user);
            return response;
        }

        [HttpGet("search/{username}")]
        public async Task<IActionResult> SearchByUsernameAsync(string username)
        {
            var response = await _userService.SearchByUsernameAsync(username);
            return response;
        }

        [HttpPatch("me")]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> UpdateMyProfileAsync(UserPutDto request)
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var response = await _userService.UpdateMyProfileAsync(user, request);
            return response;
        }

        [HttpPut("me/set-last-seen")]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> SetLastSeenAsync()
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var response = await _userService.SetLastSeenAsync(user);
            return response;
        }

        [HttpPatch("me/password")]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> UpdateMyPasswordAsync(UserPutPasswordDto request)
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var response = await _userService.UpdateMyPasswordAsync(user, request);
            return response;
        }

        [HttpDelete("me")]
        [EnsureEntityExistsFilter(typeof(User))]
        public async Task<IActionResult> DeleteMeAsync()
        {
            var user = HttpContext.Items["RequestData/User"] as User;

            var response = await _userService.DeleteMeAsync(user);
            return response;
        }
    }
}
