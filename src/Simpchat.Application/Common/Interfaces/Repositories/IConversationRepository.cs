using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Repositories
{
    public interface IConversationRepository
    {
        Task<ICollection<UserChatResponseDto>?> GetUserConversationsAsync(Guid currentUserId);
    }
}
