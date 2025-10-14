using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset DeletedAt { get; set; }
        public Guid UserId1 { get; set; }
        public Guid UserId2 { get; set; }
        public User User1 { get; set; }
        public User User2 { get; set; }
        public Chat Chat { get; set; }
    }
}
