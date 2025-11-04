using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Channel = Simpchat.Domain.Entities.Channel;

namespace Simpchat.Domain.Entities
{
    public class ChannelSubscriber : BaseEntity
    {
        public Guid ChannelId { get; set; }
        public Guid UserId { get; set; }
        public Channel Channel { get; set; }
        public User User { get; set; }
        public DateTimeOffset SubscribedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
