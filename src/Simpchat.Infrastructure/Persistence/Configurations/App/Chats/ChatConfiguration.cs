using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.AppConfigs.Chats
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
                .HasDefaultValue(ChatPrivacyType.Public)
                .IsRequired();
            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        }
    }
}
