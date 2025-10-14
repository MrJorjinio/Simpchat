using Simpchat.Domain.Entities.Channels;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Channel
    {
        public Guid Id { get; set; }
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatedById { get; set; }
        public Chat Chat { get; set; }
        public User Owner { get; set; }
        public ICollection<ChannelSubscriber> Subscribers { get; set; }
    }
}
