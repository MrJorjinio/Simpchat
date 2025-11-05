using Simpchat.Domain.Common;

namespace Simpchat.Domain.Entities
{
    public class GlobalPermission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<GlobalRolePermission> GlobalRoles { get; set; }
    }
}
