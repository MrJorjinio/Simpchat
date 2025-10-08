using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts
{
    public class UserProfilePutDto
    {
        public string Username { get; set; }
        public string Description { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public ChatMemberAddPermissionType ChatMemberAddPermissionType { get; set; }
    }
}
