using Simpchat.Domain.Common;
using Simpchat.Domain.Enums;

namespace Simpchat.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public DateTimeOffset RegisteredAt { get; set; } = DateTimeOffset.UtcNow;
        public ChatMemberAddPermissionType ChatMemberAddPermissionType { get; set; }
        public DateTimeOffset LastSeen { get; set; } = DateTimeOffset.UtcNow;
        public ICollection<MessageReaction> MessageReactions { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Channel> CreatedChannels { get; set; }
        public ICollection<Group> CreatedGroups { get; set; }
        public ICollection<Conversation> ConversationsAsUser1 { get; set; }
        public ICollection<Conversation> ConversationsAsUser2 { get; set; }
        public ICollection<ChannelSubscriber> SubscribedChannels { get; set; }
        public ICollection<GroupMember> ParticipatedGroups { get; set; }
        public ICollection<ChatUserPermission> ChatPermissions { get; set; }
        public ICollection<GlobalRoleUser> GlobalRoles { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<ChatBan> ChatBans { get; set; }
        public ICollection<UserOtp> UserOtps { get; set; }
    }   
}
