using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public ChatTypes Type { get; set; }
        public ICollection<Message> Messages { get; set; }
        public Group Group { get; set; }
        public Conversation Conversation { get; set; }
        public Channel Channel { get; set; }
    }
}
