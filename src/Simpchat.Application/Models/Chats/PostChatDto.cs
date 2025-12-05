using Simpchat.Application.Models.Files;
using Simpchat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Chats
{
    public class PostChatDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid OwnerId { get; set; }
        public ChatPrivacyTypes PrivacyType { get; set; } = ChatPrivacyTypes.Private;
    }
}
