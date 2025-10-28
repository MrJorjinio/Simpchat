using Simpchat.Application.Models.Chats.Get.UserChat;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories.Old
{
    public interface IConversationRepository
    {
        Task<ICollection<UserChatResponseDto>?> GetUserConversationsAsync(Guid currentUserId);
        Task DeleteAsync(User user1, User user2);
    }
}
