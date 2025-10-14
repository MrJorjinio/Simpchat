using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.AppConfigs.Messages
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Property(m => m.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.HasOne(m => m.Sender)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.SenderId);
            builder.Property(m => m.Content)
                .HasMaxLength(1000);
            builder.Property(m => m.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
            builder.HasOne(m => m.ReplyTo)
                .WithMany(r => r.Replies)
                .HasForeignKey(m => m.ReplyId);
        }
    }
}
