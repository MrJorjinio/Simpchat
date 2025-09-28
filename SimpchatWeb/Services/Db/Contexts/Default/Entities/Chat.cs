using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public ChatTypes Type { get; set; }
        public ChatPrivacyType PrivacyType { get; set; }
        public ICollection<Message> Messages { get; set; }
        public Group Group { get; set; }
        public Conversation Conversation { get; set; }
        public Channel Channel { get; set; }
    }
}
