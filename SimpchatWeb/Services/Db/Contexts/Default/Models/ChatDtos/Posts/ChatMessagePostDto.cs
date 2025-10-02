namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Posts
{
    public class ChatMessagePostDto
    {
        public string Content { get; set; }
        public Guid? ReplyId { get; set; }
        public string? MediaUrl { get; set; }
    }
}
