namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GroupParticipant
    {
        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
    }
}
