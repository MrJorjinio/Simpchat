using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.Chats
{
    internal class ChatPermissionConfiguration : IEntityTypeConfiguration<ChatPermission>
    {
        public void Configure(EntityTypeBuilder<ChatPermission> builder)
        {
            builder.Property(cp => cp.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.Property(cp => cp.Name)
                .HasMaxLength(85)
                .IsRequired();
            builder.HasIndex(cp => cp.Name)
                .IsUnique();
        }
    }
}
