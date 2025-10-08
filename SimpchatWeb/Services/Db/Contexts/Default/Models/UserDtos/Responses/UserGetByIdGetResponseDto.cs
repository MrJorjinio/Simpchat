using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses
{
    public class UserGetByIdGetResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool IsOnline { get; set; }
        public DateTimeOffset? LastSeen { get; set; }
    }
}
