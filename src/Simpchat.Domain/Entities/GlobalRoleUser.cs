namespace Simpchat.Domain.Entities
{
    public class GlobalRoleUser : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public GlobalRole Role { get; set; }
        public User User { get; set; }
    }
}
