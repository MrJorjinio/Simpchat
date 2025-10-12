namespace Simpchat.Domain.Entities.Chats
{
    public class Reaction
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<MessageReaction> MessagesAppliedTo { get; set; }
    }
}
