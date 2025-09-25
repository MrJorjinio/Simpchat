namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class ChannelSubscriber
    {
        public Guid UserId { get; set; }
        public Guid ChannelId { get; set; }
        public User User { get; set; }
        public Channel Channel { get; set; }
    }
}
