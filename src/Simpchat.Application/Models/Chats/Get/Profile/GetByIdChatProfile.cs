using Simpchat.Application.Models.Users.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Chats.Get.Profile
{
    public class GetByIdChatProfile
    {
        public Guid ChatId { get; set; }
        public int ParticipantsCount { get; set; }
        public int ParticipantsOnline { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Description { get; set; }
        public ICollection<UserResponseDto> Participants { get; set; }
    }
}
