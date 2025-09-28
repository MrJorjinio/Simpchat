namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GlobalRoleUser
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public GlobalRole Role { get; set; }
        public User User { get; set; }
    }
}
