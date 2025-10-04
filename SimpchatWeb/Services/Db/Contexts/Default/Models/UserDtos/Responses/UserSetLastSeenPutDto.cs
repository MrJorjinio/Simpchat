namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses
{
    public class UserSetLastSeenPutDto
    {
        public string Username { get; set; }
        public DateTimeOffset LastSeen { get; set; }
    }
}
