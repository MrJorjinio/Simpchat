using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses
{
    public class ChatSearchGetResponseDto
    {
        public Guid Id { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Name { get; set; }
        public ChatType Type { get; set; }
    }
}
