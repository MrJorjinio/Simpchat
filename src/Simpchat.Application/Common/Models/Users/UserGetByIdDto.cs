using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Models.Users
{
    public class UserGetByIdDto
    {
        public Guid UserId { get; set; }
        public Guid? ChatId { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
        public DateTimeOffset LastSeen { get; set; }
    }
}
