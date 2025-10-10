namespace Simpchat.Infrastructure.Identity
{
    public class GlobalRolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public GlobalRole Role { get; set; }
        public GlobalPermission Permission { get; set; }
    }
}
