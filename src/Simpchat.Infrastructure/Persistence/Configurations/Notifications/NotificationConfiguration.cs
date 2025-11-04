using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification = Simpchat.Domain.Entities.Notification;

namespace Simpchat.Infrastructure.Persistence.Configurations.Notifications
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
