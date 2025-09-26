using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default
{
    public class SimpchatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<MessageReaction> MessagesReactions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupUserRole> GroupsUsersRoles { get; set; }
        public DbSet<GroupUserPermission> GroupsUsersPermissions { get; set; }
        public DbSet<GroupRolePermission> GroupRolesPermissions { get; set; }
        public DbSet<GroupRole> GroupRoles { get; set; }
        public DbSet<GroupParticipant> GroupsParticipants { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<ConversationMember> ConversationsMembers { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChannelSubscriber> ChannelsSubscribers { get; set; }
        public DbSet<Channel> Channels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=simpchat;Username=postgres;Password=javohir04");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //<Enums>
            modelBuilder.HasPostgresEnum<ChatTypes>();
            modelBuilder.HasPostgresEnum<FriendshipsStatus>();
            //<.Enums>
            //<User>
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasMaxLength(20)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Description)
                .HasMaxLength(85);
            //</User>
            //<Session>
            modelBuilder.Entity<Session>()
                .Property(s => s.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<Session>()
                .Property(s => s.Device)
                .HasMaxLength(150);
            //</Session>
            //<Reaction>
            modelBuilder.Entity<Reaction>()
                .Property(r => r.Name)
                .HasMaxLength(20);
            //</Reaction>
            //<MessageReaction>
            modelBuilder.Entity<MessageReaction>()
                .HasKey(mr => new { mr.MessageId, mr.ReactionId, mr.UserId });
            //</MessageReaction>
            //<Message>
            modelBuilder.Entity<Message>()
                .Property(m => m.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.SenderId);
            modelBuilder.Entity<Message>()
                .Property(m => m.Content)
                .HasMaxLength(1000);
            //</Message>
            //<GroupUserRole>
            modelBuilder.Entity<GroupUserRole>()
                .HasKey(gur => new { gur.GroupId, gur.RoleId, gur.UserId });
            //</GroupUserRole>
            //<GroupUserPermission>
            modelBuilder.Entity<GroupUserPermission>()
                .HasKey(gup => new { gup.UserId, gup.GroupId, gup.PermissionId });
            modelBuilder.Entity<GroupUserPermission>()
                .HasOne(gup => gup.Permission)
                .WithMany(p => p.UsersAppliedTo)
                .HasForeignKey(gup => gup.PermissionId);
            //</GroupUserPermission>
            //<GroupRolePermission>
            modelBuilder.Entity<GroupRolePermission>()
                .Property(grp => grp.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<GroupRolePermission>()
                .Property(grp => grp.Name)
                .HasMaxLength(85);
            modelBuilder.Entity<GroupRolePermission>()
                .HasOne(grp => grp.RoleBelongTo)
                .WithMany(r => r.Permissions)
                .HasForeignKey(grp => grp.RoleId);
            //</GroupRolePermission>
            //<GroupRole>
            modelBuilder.Entity<GroupRole>()
                .Property(gr => gr.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<GroupRole>()
                .Property(gr => gr.Name)
                .HasMaxLength(35);
            //</GroupRole>
            //<GroupParticipant>
            modelBuilder.Entity<GroupParticipant>()
                .HasKey(gp => new { gp.GroupId, gp.UserId });
            //</GroupParticipant>
            //<Group>
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Chat)
                .WithOne(c => c.Group)
                .HasForeignKey<Group>(g => g.Id);
            modelBuilder.Entity<Group>()
                .HasKey(g => g.Id);
            modelBuilder.Entity<Group>()
                .HasOne(g => g.UserCreated)
                .WithMany(u => u.Groups)
                .HasForeignKey(g => g.CreatedById);
            modelBuilder.Entity<Group>()
                .Property(g => g.Name)
                .HasMaxLength(50);
            modelBuilder.Entity<Group>()
                .Property(g => g.Description)
                .HasMaxLength(200);
            //</Group>
            //<Friendship>
            modelBuilder.Entity<Friendship>()
                .HasKey(f => new { f.UserId, f.FriendId });
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany(u => u.ReceivedFriendships)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany(u => u.SentFriendships)
                .HasForeignKey(u => u.FriendId);
            //</Friendship>
            //<ConversationMember>
            modelBuilder.Entity<ConversationMember>()
                .HasKey(cm => new { cm.ConversationId, cm.UserId });
            //</ConversationMember>
            //<Conversation>
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Chat)
                .WithOne(c => c.Conversation)
                .HasForeignKey<Conversation>(c => c.Id);
            modelBuilder.Entity<Conversation>()
                .HasKey(c => c.Id);
            //</Conversation>
            //<Chat>
            modelBuilder.Entity<Chat>()
                .Property(c => c.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            //</Chat>
            //<ChannelSubscriber>
            modelBuilder.Entity<ChannelSubscriber>()
                .HasKey(cs => new { cs.UserId, cs.ChannelId });
            //</ChannelSubscriber>
            //<Channel>
            modelBuilder.Entity<Channel>()
                .HasOne(c => c.Chat)
                .WithOne(c => c.Channel)
                .HasForeignKey<Channel>(c => c.Id);
            modelBuilder.Entity<Channel>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Channel>()
                .HasOne(c => c.User)
                .WithMany(c => c.Channels)
                .HasForeignKey(c => c.CreatedById);
            modelBuilder.Entity<Channel>()
                .Property(c => c.Name)
                .HasMaxLength(50);
            modelBuilder.Entity<Channel>()
                .Property(c => c.Description)
                .HasMaxLength(200);
            //</Channel>
        }
    }
}
