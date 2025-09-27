namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GroupPermission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<GroupRolePermission> RolesBelongTo { get; set; }
        public ICollection<GroupUserPermission> UsersAppliedTo { get; set; }
    }
}
