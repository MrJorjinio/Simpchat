using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class ConversationMember
    {
        public ChatParticipantStatus Status { get; set; } = ChatParticipantStatus.Joined;
        public Guid UserId { get; set; }
        public Guid ConversationId { get; set; }
        public User User { get; set; }
        public Conversation Conversation { get; set; }
    }
}
