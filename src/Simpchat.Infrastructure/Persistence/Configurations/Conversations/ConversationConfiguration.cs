using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;

namespace Simpchat.Infrastructure.Persistence.Configurations.Conversations
{
    internal class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasOne(c => c.Chat)
                .WithOne(c => c.Conversation)
                .HasForeignKey<Conversation>(c => c.Id);

            builder.HasOne(c => c.User1)
              .WithMany(c => c.ConversationsAsUser1)
              .HasForeignKey(c => c.UserId1);

            builder.HasOne(u => u.User2)
                   .WithMany(c => c.ConversationsAsUser2)
                   .HasForeignKey(c => c.UserId2);

            builder.HasKey(c => new { c.Id, c.UserId1, c.UserId2 });
        }
    }
}
