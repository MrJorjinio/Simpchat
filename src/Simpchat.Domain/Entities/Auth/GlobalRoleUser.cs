using Simpchat.Domain.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;

namespace Simpchat.Infrastructure.Identity
{
    public class GlobalRoleUser : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public GlobalRole Role { get; set; }
        public User User { get; set; }
    }
}
