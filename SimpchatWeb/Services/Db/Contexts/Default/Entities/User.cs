using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public ChatMemberAddPermissionType ChatMemberAddPermissionType { get; set; }
        public DateTimeOffset RegisteredAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastSeen { get; set; } = DateTimeOffset.UtcNow;
        public ICollection<ChatParticipant> ChatsParticipated { get; set; }
        public ICollection<MessageReaction> MessageReactions { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Channel> Channels { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<ChatUserPermission> ChatPermissions { get; set; }
        public ICollection<GlobalRoleUser> GlobalRoles { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<ChatBan> ChatBans { get; set; }
    }   
}
