using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string PasswordHash { get; set; }
        public DateTimeOffset RegisteredAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastSeen { get; set; }
        public ICollection<Friendship> SentFriendships { get; set; }

        public ICollection<Friendship> ReceivedFriendships { get; set; }
        public ICollection<GroupParticipant> GroupsParticipated { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<MessageReaction> MessageReactions { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Channel> Channels { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<ChannelSubscriber> ChannelsSubscribedTo { get; set; }
        public ICollection<GroupUserRole> GroupsRoles { get; set; }
        public ICollection<GroupUserPermission> GroupPermissions { get; set; }
    }   
}
