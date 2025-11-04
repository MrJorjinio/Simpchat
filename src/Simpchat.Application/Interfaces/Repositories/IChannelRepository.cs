using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IChannelRepository : IBaseRepository<Channel>, ISearchableRepository<Channel>
    {
        Task AddSubscriberAsync(Guid userId, Guid channelId);
        Task DeleteSubscriberAsync(ChannelSubscriber channelSubscriber);
        Task<List<Channel>> GetUserSubscribedChannelsAsync(Guid userId);
    }
}
