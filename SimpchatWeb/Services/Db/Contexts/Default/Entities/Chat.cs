using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public ChatTypes Type { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<Conversation> Conversations { get; set; }
        public ICollection<Channel> Channels { get; set; }
    }
}
