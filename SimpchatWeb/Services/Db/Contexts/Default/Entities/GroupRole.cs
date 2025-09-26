namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GroupRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<GroupUserRole> UsersAppliedTo { get; set; }
        public ICollection<GroupRolePermission> Permissions { get; set; }
    }
}
