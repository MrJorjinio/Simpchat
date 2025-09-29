using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default
{
    public class SimpchatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<MessageReaction> MessagesReactions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupUserRole> GroupsUsersRoles { get; set; }
        public DbSet<GroupUserPermission> GroupsUsersPermissions { get; set; }
        public DbSet<GroupRolePermission> GroupRolesPermissions { get; set; }
        public DbSet<GroupRole> GroupRoles { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }
        public DbSet<GroupParticipant> GroupsParticipants { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<ConversationMember> ConversationsMembers { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChannelSubscriber> ChannelsSubscribers { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<GlobalRole> GlobalRoles { get; set; }
        public DbSet<GlobalPermission> GlobalPermissions { get; set; }
        public DbSet<GlobalRolePermission> GlobalRolesPermissions { get; set; }
        public DbSet<GlobalRoleUser> GlobalRolesUsers { get; set; }

        public SimpchatDbContext(DbContextOptions<SimpchatDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //<Enums>
            modelBuilder.HasPostgresEnum<ChatTypes>();
            modelBuilder.HasPostgresEnum<ChatPrivacyType>();
            modelBuilder.HasPostgresEnum<ChatParticipantStatus>();
            modelBuilder.HasPostgresEnum<FriendshipsStatus>();
            modelBuilder.HasPostgresEnum<UserStatus>();
            //<Enums>
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
                .HasMaxLength(85)
                .HasDefaultValue(string.Empty);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasConversion<string>()
                .HasDefaultValue(UserStatus.Active)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.RegisteredAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.LastSeen)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
            //</User>
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
            //<GroupPermission>
            modelBuilder.Entity<GroupPermission>()
                .Property(grp => grp.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<GroupPermission>()
                .Property(grp => grp.Name)
                .HasMaxLength(85)
                .IsRequired();
            modelBuilder.Entity<GroupPermission>()
                .HasIndex(gp => gp.Name)
                .IsUnique();
            //</GroupPermission>
            //<GroupRole>
            modelBuilder.Entity<GroupRole>()
                .Property(gr => gr.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<GroupRole>()
                .Property(gr => gr.Name)
                .HasMaxLength(35)
                .IsRequired();
            modelBuilder.Entity<GroupRole>()
                .HasIndex(gr => gr.Name)
                .IsUnique();
            //</GroupRole>
            //</GroupRolePermission>
            modelBuilder.Entity<GroupRolePermission>()
                .HasKey(grp => new { grp.RoleId, grp.PermissionId });
            //</GroupRolePermission>
            //<GlobalPermission>
            modelBuilder.Entity<GlobalPermission>()
                .Property(gp => gp.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<GlobalPermission>()
                .Property(gp => gp.Name)
                .HasMaxLength(85)
                .IsRequired();
            modelBuilder.Entity<GlobalPermission>()
                .HasIndex(gp => gp.Name)
                .IsUnique();
            modelBuilder.Entity<GlobalPermission>()
                .Property(gp => gp.Description)
                .HasMaxLength(250);
            //</GlobalPermission>
            //<GlobalRole>
            modelBuilder.Entity<GlobalRole>()
                .Property(gr => gr.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<GlobalRole>()
                .Property(gr => gr.Name)
                .HasMaxLength(35)
                .IsRequired();
            modelBuilder.Entity<GlobalRole>()
                .Property(gr => gr.Description)
                .HasMaxLength(250);
            modelBuilder.Entity<GlobalRole>()
                .HasIndex(gr => gr.Name)
                .IsUnique();
            //</GlobalRole>
            //<GlobalRolePermission>
            modelBuilder.Entity<GlobalRolePermission>()
                .HasKey(grp => new { grp.RoleId, grp.PermissionId });
            //</GlobalRolePermission>
            //<GlobalRoleUser>
            modelBuilder.Entity<GlobalRoleUser>()
                .HasKey(gru => new { gru.UserId, gru.RoleId });
            //</GlobalRoleUser>
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
                .HasMaxLength(50)
                .IsRequired();
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
            modelBuilder.Entity<Friendship>()
                .Property(f => f.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Friendship>()
                .Property(f => f.FormedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
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
            modelBuilder.Entity<Chat>()
                .Property(c => c.Type)
                .HasConversion<string>()
                .IsRequired();
            modelBuilder.Entity<Chat>()
                .Property(c => c.PrivacyType)
                .HasConversion<string>()
                .HasDefaultValue(ChatPrivacyType.Public)
                .IsRequired();
            modelBuilder.Entity<Chat>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
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
            modelBuilder.Entity<Channel>()
                .Property(c => c.Name)
                .IsRequired();
            //</Channel>
        }
    }
}
