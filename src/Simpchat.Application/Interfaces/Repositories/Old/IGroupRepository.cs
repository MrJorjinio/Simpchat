using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Search;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories.Old
{
    public interface IGroupRepository
    {
        Task<ICollection<SearchChatResponseDto>?> SearchByNameAsync(string searchTerm);
        Task<ICollection<UserChatResponseDto>?> GetUserParticipatedGroupsAsync(Guid currentUserId);
        Task CreateAsync(Group group);
        Task AddMemberAsync(Chat chat, User addingUser, User currentUser);
        Task AddUserPermissionAsync(ChatPermission permission, Chat chat, User addingUser, User currentUser);
        Task DeleteMemberAsync(User user, Group group);
        Task DeleteAsync(Group group);
        Task UpdateAsync(Group group);
    }
}
