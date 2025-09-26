namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GroupUserPermission
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
        public GroupRolePermission Permission { get; set; }
    }
}
