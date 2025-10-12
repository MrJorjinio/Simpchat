using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Infrastructure.Identity;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Notification = SimpchatWeb.Services.Db.Contexts.Default.Entities.Notification;

namespace Simpchat.Infrastructure.Persistence.Configurations.AppConfigs
{
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(n => n.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.HasOne(n => n.Receiver)
                .WithMany(r => r.Notifications)
                .HasForeignKey(n => n.ReceiverId);
        }
    }
}
