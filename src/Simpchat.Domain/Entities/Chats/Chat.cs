using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public ChatType Type { get; set; }
        public ChatPrivacyType PrivacyType { get; set; }
        public Group Group { get; set; }
        public Conversation Conversation { get; set; }
        public Channel Channel { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatParticipant> Participants { get; set; }
        public ICollection<ChatBan> Bans { get; set; }
        public ICollection<ChatUserPermission> ParticipantsPermissions { get; set; }

        public string DisplayName(Guid currentUserId)
        {
            if (Group != null) return Group.Name;
            if (Channel != null) return Channel.Name;

            return Participants
                .FirstOrDefault(p => p.UserId != currentUserId)?
                .User?.Username ?? "Unknown";
        }

        public string DisplayProfilePicUrl(Guid currentUserId)
        {
            if (Group != null) return Group.ProfilePictureUrl;
            if (Channel != null) return Channel.ProfilePictureUrl;

            return Participants
                .FirstOrDefault(p => p.UserId != currentUserId)?
                .User?.ProfilePictureUrl ?? "";
        }
    }
}
