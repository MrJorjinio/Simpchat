using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Infrastructure.Identity;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.AppConfigs.Conversations
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
