using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.Channels
{
    internal class ChannelConfiguration : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder.HasOne(c => c.Chat)
                .WithOne(c => c.Channel)
                .HasForeignKey<Channel>(c => c.Id);
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.Owner)
                .WithMany(c => c.CreatedChannels)
                .HasForeignKey(c => c.CreatedById);
            builder.Property(c => c.Name)
                .HasMaxLength(50);
            builder.Property(c => c.Description)
                .HasMaxLength(200);
            builder.Property(c => c.Name)
                .IsRequired();
            builder.Property(c => c.AvatarUrl)
                .IsRequired(false);
        }
    }
}
