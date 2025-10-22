using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Search;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IChannelRepository
    {
        Task<ICollection<SearchChatResponseDto>?> SearchByNameAsync(string searchTerm);
        Task<ICollection<UserChatResponseDto>?> GetUserSubscribedChannelsAsync(Guid currentUserId);
        Task CreateAsync(Channel channel);
        Task AddSubscriberAsync(Chat chat, User addingUser, User currentUser);
        Task AddUserPermissionAsync(ChatPermission permission, Chat chat, User addingUser, User currentUser);
        Task DeleteSubscriberAsync(User user, Channel channel);
        Task DeleteAsync(Channel channel);
    }
}
