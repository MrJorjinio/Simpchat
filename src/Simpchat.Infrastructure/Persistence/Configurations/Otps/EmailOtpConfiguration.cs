using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.Emails
{
    internal class EmailOtpConfiguration : IEntityTypeConfiguration<EmailOtp>
    {
        public void Configure(EntityTypeBuilder<EmailOtp> builder)
        {
            builder.Property(u => u.Id)
              .HasDefaultValueSql("gen_random_uuid()");
        }
    }
}
