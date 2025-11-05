using Simpchat.Domain.Common;

namespace Simpchat.Domain.Entities
{
    public class ChatPermission : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ChatUserPermission> UsersAppliedTo { get; set; }
    }
}
