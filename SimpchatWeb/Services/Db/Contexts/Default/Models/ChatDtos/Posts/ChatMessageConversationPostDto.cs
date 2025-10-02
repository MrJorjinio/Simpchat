namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Posts
{
    public class ChatMessageConversationPostDto
    {
        public Guid? ReceiverId { get; set; } = null;
        public string Content { get; set; }
        public Guid? ReplyId { get; set; } = null;
        public string? MediaUrl { get; set; } = null;
    }
}
