using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses
{
    public class ChatPrivacyTypeUpdatePutDto
    {
        public Guid ChatId { get; set; }
        public ChatPrivacyType PrivacyType { get; set; }
    }
}
