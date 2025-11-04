namespace Simpchat.Domain.Entities
{
    public class ChatBan : BaseEntity
    {
        public DateTimeOffset From { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset To { get; set; }
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public Chat Chat { get; set; }
        public User User { get; set; }
        
    }
}
