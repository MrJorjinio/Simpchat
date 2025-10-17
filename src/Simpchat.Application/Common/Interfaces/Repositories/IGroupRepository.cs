using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Application.Common.Models.Chats.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Repositories
{
    public interface IGroupRepository
    {
        Task<ICollection<ChatSearchResponseDto>?> SearchByNameAsync(string searchTerm);
        Task<ICollection<UserChatResponseDto>?> GetUserParticipatedGroupsAsync(Guid currentUserId);
    }
}
