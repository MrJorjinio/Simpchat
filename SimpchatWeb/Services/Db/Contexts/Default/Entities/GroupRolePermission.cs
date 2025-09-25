namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GroupRolePermission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid RoleId { get; set; }
        public GroupRole RoleBelongTo { get; set; }
        public ICollection<GroupUserPermission> UsersAppliedTo { get; set; }
    }
}
