using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserNotificationDtos.Responses
{
    public class UserNotificationGetResponseDto
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public string Content { get; set; }
        public string ChatName { get; set; }
        public string SenderName { get; set; }
        public ChatType ChatType { get; set; }
    }
}
