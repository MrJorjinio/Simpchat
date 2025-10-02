using static System.Net.Mime.MediaTypeNames;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid? ReplyId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChatId { get; set; }
        public User Sender { get; set; }
        public Chat Chat { get; set; }
        public Message ReplyTo { get; set; }
        public ICollection<MessageReaction> Reactions { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Message> Replies { get; set; }
    }
}
