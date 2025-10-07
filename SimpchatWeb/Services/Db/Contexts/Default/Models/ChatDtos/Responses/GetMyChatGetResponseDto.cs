using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses
{
    public class GetMyChatGetResponseDto
    {
        public Guid Id { get; set; }
        public ChatType Type { get; set; }
        public string Name { get; set; }
        public ChatMessageGetByIdGetResponseDto LastMessage { get; set; }
        public DateTimeOffset LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
