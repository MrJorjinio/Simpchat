using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.IdentityConfigs
{
    internal class GlobalRoleConfiguration : IEntityTypeConfiguration<GlobalRole>
    {
        public void Configure(EntityTypeBuilder<GlobalRole> builder)
        {
            builder.Property(gr => gr.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.Property(gr => gr.Name)
                .HasMaxLength(35)
                .IsRequired();
            builder.Property(gr => gr.Description)
                .HasMaxLength(250);
            builder.HasIndex(gr => gr.Name)
                .IsUnique();
        }
    }
}
