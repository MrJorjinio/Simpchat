namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Chat Chat { get; set; }
        public Friendship Friendship { get; set; }
        public ICollection<ConversationMember> Members { get; set; }
    }
}
