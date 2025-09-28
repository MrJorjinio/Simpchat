namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GroupRolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public GroupRole Role { get; set; }
        public GroupPermission Permission { get; set; }
    }
}
