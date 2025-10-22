using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Chats.Get.UserChat
{
    public class UserChatResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public ChatType Type { get; set; }
        public int NotificationsCount { get; set; }
        public LastMessageResponseDto? LastMessage { get; set; }
        public DateTimeOffset? UserLastMessage { get; set; }
    }
}
