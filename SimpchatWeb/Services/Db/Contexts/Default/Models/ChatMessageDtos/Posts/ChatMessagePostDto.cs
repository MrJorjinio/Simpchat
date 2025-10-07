namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Posts
{
    public class ChatMessagePostDto
    {
        public Guid? ReceiverId { get; set; } = null;
        public string Content { get; set; }
        public Guid? ReplyId { get; set; } = null;
        public string? MediaUrl { get; set; } = null;
    }
}
