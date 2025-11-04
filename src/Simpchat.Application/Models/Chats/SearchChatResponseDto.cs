using Simpchat.Domain.Enums;

namespace Simpchat.Application.Models.Chats
{
    public class SearchChatResponseDto
    {
        public Guid EntityId { get; set; }
        public Guid? ChatId { get; set; }
        public ChatType ChatType { get; set; }
        public string DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
