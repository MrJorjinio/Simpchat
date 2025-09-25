namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class GroupUserRole
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public Guid RoleId { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
        public GroupRole Role { get; set; }
    }
}
