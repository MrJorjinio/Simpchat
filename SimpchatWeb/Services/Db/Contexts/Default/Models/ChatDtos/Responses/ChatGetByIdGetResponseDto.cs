using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses
{
    public class ChatGetByIdGetResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public int MembersCount { get; set; }
        public int MembersOnline { get; set; }
        public ICollection<ChatMessageGetByIdGetResponseDto> Messages { get; set; }
    }
}
