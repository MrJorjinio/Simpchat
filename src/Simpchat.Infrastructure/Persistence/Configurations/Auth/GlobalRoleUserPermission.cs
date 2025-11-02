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
    internal class GlobalRoleUserPermission : IEntityTypeConfiguration<GlobalRoleUser>
    {
        public void Configure(EntityTypeBuilder<GlobalRoleUser> builder)
        {
            builder.Property(gru => gru.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.HasKey(gra => new { gra.UserId, gra.RoleId, gra.Id });
        }
    }
}
