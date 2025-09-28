using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GroupParticipant
    {
        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LeaveAt { get; set; }
        public ChatParticipantStatus ParticipantStatus { get; set; } = ChatParticipantStatus.Joined;
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
    }
}
