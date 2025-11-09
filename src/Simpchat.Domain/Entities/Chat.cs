using Simpchat.Domain.Common;
using Simpchat.Domain.Enums;

namespace Simpchat.Domain.Entities
{
    public class Chat : BaseEntity
    {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public ChatTypes Type { get; set; }
        public ChatPrivacyType PrivacyType { get; set; }
        public Group? Group { get; set; }
        public Conversation? Conversation { get; set; }
        public Channel? Channel { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public ICollection<ChatBan>? Bans { get; set; }
        public ICollection<ChatUserPermission> ParticipantsPermissions { get; set; }
    }
}
