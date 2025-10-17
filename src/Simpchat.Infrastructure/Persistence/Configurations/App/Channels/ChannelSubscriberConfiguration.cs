﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simpchat.Domain.Entities.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Configurations.App.Channels
{
    internal class ChannelSubscriberConfiguration : IEntityTypeConfiguration<ChannelSubscriber>
    {
        public void Configure(EntityTypeBuilder<ChannelSubscriber> builder)
        {
            builder.HasKey(cs => new { cs.UserId, cs.ChannelId });
        }
    }
}
