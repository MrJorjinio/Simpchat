namespace Simpchat.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public bool IsSeen { get; set; }
        public Guid MessageId { get; set; }
        public Guid ReceiverId { get; set; }
        public Message Message { get; set; }
        public User Receiver { get; set; }
    }
}
