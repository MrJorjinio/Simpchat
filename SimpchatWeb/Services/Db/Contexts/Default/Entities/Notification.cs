namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public bool IsSeen { get; set; }
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public Message Message { get; set; }
        public User User { get; set; }
    }
}
