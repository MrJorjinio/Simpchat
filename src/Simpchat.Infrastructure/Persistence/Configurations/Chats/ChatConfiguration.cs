using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Enums;

namespace Simpchat.Infrastructure.Persistence.Configurations.Chats
{
    internal class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.Property(c => c.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.Property(c => c.Type)
                .HasConversion<string>()
                .IsRequired();
            builder.Property(c => c.PrivacyType)
                .HasConversion<string>()
                .HasDefaultValue(ChatPrivacyTypes.Public)
                .IsRequired();
            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        }
    }
}
