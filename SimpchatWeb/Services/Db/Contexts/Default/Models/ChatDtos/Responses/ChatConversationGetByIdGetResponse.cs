using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses
{
    public class ChatConversationGetByIdGetResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public DateTimeOffset LastSeen { get; set; }
        public ICollection<ChatMessageGetByIdGetResponseDto> Messages { get; set; }
    }
}
