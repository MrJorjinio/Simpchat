namespace Simpchat.Application.Models.Permissions
{
    public class RevokePermissionDto
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
        public string PermissionName { get; set; }
    }
}
