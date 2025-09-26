using static System.Net.Mime.MediaTypeNames;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Channel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid CreatedById { get; set; }
        public User User { get; set; }
        public Chat Chat { get; set; }
        public ICollection<ChannelSubscriber> Subscribers { get; set; }
    }
}
