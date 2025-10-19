using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Application.Common.Models.Chats.Search;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Repositories
{
    public interface IChannelRepository
    {
        Task<ICollection<ChatSearchResponseDto>?> SearchByNameAsync(string searchTerm);
        Task<ICollection<UserChatResponseDto>?> GetUserSubscribedChannelsAsync(Guid currentUserId);
        Task CreateAsync(Channel channel);
        Task AddSubscriberAsync(Chat chat, User addingUser, User currentUser);
        Task AddUserPermissionAsync(ChatPermission permission, Chat chat, User addingUser, User currentUser);
    }
}
