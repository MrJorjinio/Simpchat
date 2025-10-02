namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Temps
{
    public class ChatMessageTempDto
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
        public string Content { get; set; }
        public bool IsSeen { get; set; }
    }
}
