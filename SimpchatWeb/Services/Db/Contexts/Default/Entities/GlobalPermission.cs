namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GlobalPermission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<GlobalRolePermission> GlobalRoles { get; set; }
    }
}
