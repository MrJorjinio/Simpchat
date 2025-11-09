using Simpchat.Application.Models.Messages;
using Simpchat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Chats
{
    public class GetByIdChatDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? AvatarUrl { get; set; }
        public ChatTypes Type { get; set; }
        public int ParticipantsCount { get; set; }
        public int ParticipantsOnline { get; set; }
        public int NotificationsCount { get; set; }
        public ICollection<GetByIdMessageDto> Messages { get; set; }
    }
}
