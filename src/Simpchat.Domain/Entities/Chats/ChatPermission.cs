using Simpchat.Domain.Entities;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class ChatPermission : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ChatUserPermission> UsersAppliedTo { get; set; }
    }
}
