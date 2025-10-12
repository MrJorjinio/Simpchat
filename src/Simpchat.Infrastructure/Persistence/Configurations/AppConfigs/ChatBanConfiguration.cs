using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities.Chats;
using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.AppConfigs
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
