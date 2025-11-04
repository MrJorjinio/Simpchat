using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;

namespace Simpchat.Infrastructure.Persistence.Configurations.Chats
{
    internal class ChatBanConfiguration : IEntityTypeConfiguration<ChatBan>
    {
        public void Configure(EntityTypeBuilder<ChatBan> builder)
        {
            builder.Property(cp => cp.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.Property(cp => cp.From)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        }
    }
}
