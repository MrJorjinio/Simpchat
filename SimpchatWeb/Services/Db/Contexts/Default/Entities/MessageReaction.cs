namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class MessageReaction
    {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid UserId { get; set; }
        public Guid ReactionId { get; set; }
        public Guid MessageId { get; set; }
        public User User { get; set; }
        public Reaction Reaction { get; set; }
        public Message Message { get; set; }
    }
}
