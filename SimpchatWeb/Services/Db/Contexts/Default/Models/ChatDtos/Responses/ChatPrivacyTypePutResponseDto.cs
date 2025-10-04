using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using System.Text.Json.Serialization;

namespace SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses
{
    public class ChatPrivacyTypePutResponseDto
    {
        public ChatPrivacyType PrivacyType { get; set; }
    }
}
