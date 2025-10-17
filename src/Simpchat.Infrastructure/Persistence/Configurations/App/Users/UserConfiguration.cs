using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.AppConfigs.Users
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Id)
               .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(u => u.Username)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.Description)
                .IsRequired(false)
                .HasMaxLength(85)
                .HasDefaultValue(string.Empty);

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.Property(u => u.LastSeen)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            builder.Property(u => u.ChatMemberAddPermissionType)
                .HasConversion<string>();

            builder.Property(u => u.AvatarUrl)
                .IsRequired(false);
        }
    }
}
