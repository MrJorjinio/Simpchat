using Simpchat.Domain.Common;

namespace Simpchat.Domain.Entities
{
    public class GlobalRole : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<GlobalRoleUser> Users { get; set; }
        public ICollection<GlobalRolePermission> Permissions { get; set; }
    }
}
