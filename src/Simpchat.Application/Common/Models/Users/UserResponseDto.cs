using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Models.Users
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string AvatarUrl { get; set; }
        public DateTimeOffset LastSeen { get; set; }
        public bool IsOnline { get; set; }

        public static UserResponseDto ConvertFromDomainObject(User user)
        {
            if (user is null) return null;

            var response = new UserResponseDto
            {
                Id = user.Id,
                Description = user.Description,
                IsOnline = user.LastSeen.AddSeconds(10) > DateTimeOffset.UtcNow,
                LastSeen = user.LastSeen,
                AvatarUrl = user.AvatarUrl,
                Username = user.Username
            };

            return response;
        }
    }
}
