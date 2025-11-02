using Simpchat.Domain.Entities;

namespace Simpchat.Infrastructure.Identity
{
    public class GlobalPermission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<GlobalRolePermission> GlobalRoles { get; set; }
    }
}
