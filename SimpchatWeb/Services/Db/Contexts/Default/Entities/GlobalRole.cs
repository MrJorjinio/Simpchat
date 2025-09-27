namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GlobalRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<GlobalRoleUser> Users { get; set; }
        public ICollection<GlobalRolePermission> Permissions { get; set; }
    }
}
