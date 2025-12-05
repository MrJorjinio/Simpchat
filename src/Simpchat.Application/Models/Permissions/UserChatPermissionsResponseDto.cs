namespace Simpchat.Application.Models.Permissions
{
    public class UserChatPermissionsResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Guid ChatId { get; set; }
        public List<UserPermissionResponseDto> Permissions { get; set; } = new();
    }
}
