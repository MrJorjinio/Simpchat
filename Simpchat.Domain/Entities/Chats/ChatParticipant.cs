using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class ChatParticipant
    {
        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LeaveAt { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
        public User User { get; set; }
        public Chat Chat { get; set; }
    }
}
