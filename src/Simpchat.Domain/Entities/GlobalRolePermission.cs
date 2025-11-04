namespace Simpchat.Domain.Entities
{
    public class GlobalRolePermission : BaseEntity
    {
        public Guid PermissionId { get; set; }
        public GlobalRole Role { get; set; }
        public Guid RoleId { get; set; }
        public GlobalPermission Permission { get; set; }
    }
}
