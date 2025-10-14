using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Models.Chats.Search;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IChannelRepository _channelRepository;

        public ChatRepository(
            IUserRepository userRepository,
            IGroupRepository groupRepository,
            IChannelRepository channelRepository

            )
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _channelRepository = channelRepository;
        }

        public async Task<ICollection<ChatSearchResponseDto>?> SearchByNameAsync(string searchTerm, Guid currentUserId)
        {
            var users = await _userRepository.SearchByUsernameAsync(searchTerm, currentUserId);
            var groups = await _groupRepository.SearchByNameAsync(searchTerm);
            var channels = await _channelRepository.SearchByNameAsync(searchTerm);

            var result = users
                .Concat(groups)
                .Concat(channels)
                .ToList();

            return result;
        }
    }
}
