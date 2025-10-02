namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses
{
    public class UserProfileGetResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public DateTimeOffset LastSeen { get; set; }
    }
}
