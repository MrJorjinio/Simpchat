using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.AppConfigs.Groups
{
    internal class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasOne(g => g.Chat)
               .WithOne(c => c.Group)
               .HasForeignKey<Group>(g => g.Id);
            builder.HasKey(g => g.Id);
            builder.HasOne(g => g.Owner)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(g => g.CreatedById);
            builder.Property(g => g.Name)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(g => g.Description)
                .HasMaxLength(200);
        }
    }
}
