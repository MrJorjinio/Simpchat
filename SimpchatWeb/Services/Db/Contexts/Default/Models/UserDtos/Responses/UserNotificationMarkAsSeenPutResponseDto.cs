namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses
{
    public class UserNotificationMarkAsSeenPutResponseDto
    {
        public Guid MessageId { get; set; }
        public bool IsSeen { get; set; }
    }
}
