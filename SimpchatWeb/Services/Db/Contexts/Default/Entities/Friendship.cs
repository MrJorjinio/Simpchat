using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Friendship
    {
        public FriendshipsStatus Status { get; set; } = FriendshipsStatus.Pending;
        public DateTimeOffset FormedAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
        public Guid ConversationId { get; set; }
        public User User { get; set; }
        public User Friend { get; set; }
        public Conversation Conversation { get; set; }
    }
}
