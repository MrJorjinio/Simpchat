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

namespace Simpchat.Infrastructure.Persistence.Configurations.AppConfigs
{
    internal class ChatUserPermissionConfiguration : IEntityTypeConfiguration<ChatUserPermission>
    {
        public void Configure(EntityTypeBuilder<ChatUserPermission> builder)
        {
            builder.HasKey(cup => new { cup.UserId, cup.ChatId, cup.PermissionId });
            builder.HasOne(cup => cup.Permission)
                .WithMany(p => p.UsersAppliedTo)
                .HasForeignKey(cup => cup.PermissionId);
        }
    }
}
