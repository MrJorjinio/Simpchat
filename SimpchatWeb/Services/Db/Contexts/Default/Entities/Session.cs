using static System.Net.Mime.MediaTypeNames;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Session
    {
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public string Device { get; set; }
        public string IpAddress { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset ConnectedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset DisconnectedAt { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
