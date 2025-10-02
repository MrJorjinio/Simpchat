namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses
{
    public class ChatMessageResponseDto
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
        public string Content { get; set; }
        public bool IsSeen { get; set; }
    }
}
