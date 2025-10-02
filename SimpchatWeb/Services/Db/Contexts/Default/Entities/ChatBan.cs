namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class ChatBan
    {
        public Guid Id { get; set; }
        public DateTimeOffset From { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset To { get; set; }
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public Chat Chat { get; set; }
        public User User { get; set; }
        
    }
}
