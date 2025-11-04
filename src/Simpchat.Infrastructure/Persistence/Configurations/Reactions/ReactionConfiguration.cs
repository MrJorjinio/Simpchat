using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;

namespace Simpchat.Infrastructure.Persistence.Configurations.Reactions
{
    internal class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.Property(r => r.Id)
                .HasDefaultValueSql("gen_random_uuid()");
            builder.Property(r => r.Name).HasMaxLength(20);
        }
    }
}
