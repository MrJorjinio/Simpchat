using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Chats.Get.ById
{
    public class GetByIdChatDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? AvatarUrl { get; set; }
        public ChatType Type { get; set; }
        public int ParticipantsCount { get; set; }
        public int ParticipantsOnline { get; set; }
        public int NotificationsCount { get; set; }
        public ICollection<GetByIdMessageDto> Messages { get; set; }
    }
}
