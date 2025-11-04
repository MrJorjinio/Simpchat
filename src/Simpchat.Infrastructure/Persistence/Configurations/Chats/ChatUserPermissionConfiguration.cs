using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;

namespace Simpchat.Infrastructure.Persistence.Configurations.Chats
{
    internal class ChatUserPermissionConfiguration : IEntityTypeConfiguration<ChatUserPermission>
    {
        public void Configure(EntityTypeBuilder<ChatUserPermission> builder)
        {
            builder.Property(cup => cup.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.HasKey(cup => new { cup.UserId, cup.ChatId, cup.PermissionId, cup.Id });
            builder.HasOne(cup => cup.Permission)
                .WithMany(p => p.UsersAppliedTo)
                .HasForeignKey(cup => cup.PermissionId);
        }
    }
}
