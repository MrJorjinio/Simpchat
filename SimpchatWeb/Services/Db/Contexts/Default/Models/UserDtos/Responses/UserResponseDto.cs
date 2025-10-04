using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses
{
    public class UserResponseDto
    {
        public string Username { get; set; }
        public string Description { get; set; }
        public ChatMemberAddPermissionType ChatMemberAddPermissionType { get; set; }
    }
}
