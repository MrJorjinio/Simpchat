namespace Simpchat.Application.Models.Permissions
{
    public class GrantPermissionDto
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
        public string PermissionName { get; set; }
    }
}
