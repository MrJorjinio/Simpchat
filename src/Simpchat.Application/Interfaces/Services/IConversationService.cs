using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats.Get.UserChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IConversationService
    {
        Task<ApiResult<List<UserChatResponseDto>>> GetUserConversationsAsync(Guid userId);
        Task<ApiResult> DeleteAsync(Guid conversationId);
    }
}
