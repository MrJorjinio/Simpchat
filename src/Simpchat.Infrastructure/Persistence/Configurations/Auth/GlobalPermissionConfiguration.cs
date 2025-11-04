using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.IdentityConfigs
{
    internal class GlobalPermissionConfiguration : IEntityTypeConfiguration<GlobalPermission>
    {
        public void Configure(EntityTypeBuilder<GlobalPermission> builder)
        {
            builder.Property(gp => gp.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.Property(gp => gp.Name)
                .HasMaxLength(85)
                .IsRequired();
            builder.HasIndex(gp => gp.Name)
                .IsUnique();
            builder.Property(gp => gp.Description)
                .HasMaxLength(250);
        }
    }
}
