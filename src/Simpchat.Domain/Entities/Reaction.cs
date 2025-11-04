namespace Simpchat.Domain.Entities
{
    public class Reaction : BaseEntity
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<MessageReaction> MessagesAppliedTo { get; set; }
    }
}
