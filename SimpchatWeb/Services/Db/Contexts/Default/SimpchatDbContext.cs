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
        public DbSet<ChatUserPermission> ChatsUsersPermissions { get; set; }
        public DbSet<ChatPermission> ChatPermissions { get; set; }
        public DbSet<ChatParticipant> ChatsParticipants { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<GlobalRole> GlobalRoles { get; set; }
        public DbSet<GlobalPermission> GlobalPermissions { get; set; }
        public DbSet<GlobalRolePermission> GlobalRolesPermissions { get; set; }
        public DbSet<GlobalRoleUser> UsersGlobalRoles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ChatBan> ChatsBans { get; set; }

        public SimpchatDbContext(DbContextOptions<SimpchatDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //<Enums>
            modelBuilder.HasPostgresEnum<ChatTypes>();
            modelBuilder.HasPostgresEnum<ChatPrivacyType>();
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
            modelBuilder.Entity<Message>()
                .Property(m => m.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReplyTo)
                .WithMany(r => r.Replies)
                .HasForeignKey(m => m.ReplyId);
            //</Message>
            //<ChatUserPermission>
            modelBuilder.Entity<ChatUserPermission>()
                .HasKey(cup => new { cup.UserId, cup.ChatId, cup.PermissionId });
            modelBuilder.Entity<ChatUserPermission>()
                .HasOne(cup => cup.Permission)
                .WithMany(p => p.UsersAppliedTo)
                .HasForeignKey(cup => cup.PermissionId);
            //</ChatUserPermission>
            //<ChatPermission>
            modelBuilder.Entity<ChatPermission>()
                .Property(cp => cp.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<ChatPermission>()
                .Property(cp => cp.Name)
                .HasMaxLength(85)
                .IsRequired();
            modelBuilder.Entity<ChatPermission>()
                .HasIndex(cp => cp.Name)
                .IsUnique();
            //</ChatPermission>
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
            //<ChatParticipant>
            modelBuilder.Entity<ChatParticipant>()
                .HasKey(cp => new { cp.UserId, cp.ChatId });
            //</ChatParticipant>
            //<Channel>
            modelBuilder.Entity<Channel>()
                .HasOne(c => c.Chat)
                .WithOne(c => c.Channel)
                .HasForeignKey<Channel>(c => c.Id);
            modelBuilder.Entity<Channel>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Channel>()
                .HasOne(c => c.UserCreated)
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
            //<Notifications>
            modelBuilder.Entity<Notification>()
                .Property(n => n.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            //</Notifications>
            //<ChatsBans>
            modelBuilder.Entity<ChatBan>()
                .Property(cp => cp.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<ChatBan>()
                .Property(cp => cp.From)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
            //</ChatBans>
        }
    }
}
