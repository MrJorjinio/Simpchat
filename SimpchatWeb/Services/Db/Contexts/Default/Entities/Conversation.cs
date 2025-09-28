using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset DeletedAt { get; set; }
        public Chat Chat { get; set; }
        public Friendship Friendship { get; set; }
        public ICollection<ConversationMember> Members { get; set; }
    }
}
