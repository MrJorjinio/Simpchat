using Simpchat.Domain.Common;

namespace Simpchat.Domain.Entities
{
    public class MessageReaction : BaseEntity
    {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid UserId { get; set; }
        public Guid ReactionId { get; set; }
        public Guid MessageId { get; set; }
        public User User { get; set; }
        public Reaction Reaction { get; set; }
        public Message Message { get; set; }
    }
}
