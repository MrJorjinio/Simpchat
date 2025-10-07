namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses
{
    public class ChatMessageGetByIdGetResponseDto
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public bool IsSeen { get; set; }
    }
}
