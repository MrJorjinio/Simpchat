using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Chat : BaseEntity
    {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public ChatType Type { get; set; }
        public ChatPrivacyType PrivacyType { get; set; }
        public Group? Group { get; set; }
        public Conversation? Conversation { get; set; }
        public Channel? Channel { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public ICollection<ChatBan>? Bans { get; set; }
        public ICollection<ChatUserPermission> ParticipantsPermissions { get; set; }
    }
}
