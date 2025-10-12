namespace Simpchat.Web.ViewModels.Users
{
    public class UserGetByIdDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string ProfilePicUrl { get; set; }
        public DateTimeOffset? LastSeen { get; set; }
        public bool IsOnline { get; set; }
    }
}
