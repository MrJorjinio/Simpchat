using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public ChatType Type { get; set; }
        public ChatPrivacyType PrivacyType { get; set; }
        public Group Group { get; set; }
        public Conversation Conversation { get; set; }
        public Channel Channel { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatParticipant> Participants { get; set; }
        public ICollection<ChatBan> Bans { get; set; }
        public ICollection<ChatUserPermission> ParticipantsPermissions { get; set; }
    }
}
