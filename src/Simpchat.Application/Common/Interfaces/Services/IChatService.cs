using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.Chats.Messages;
using Simpchat.Application.Common.Models.Chats.Response;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Services
{
    public interface IChatService
    {
        Task<ApiResult<ChatGetByIdResponseDto>> GetByIdAsync(Guid chatId, Guid userId);
        Task<ApiResult<ICollection<ChatGetByIdResponseDto>?>> SearchByNameAsync(string searchTerm, Guid userId);
        Task<ApiResult> SendMessageAsync(ChatSendMessageDto model);
        Task<ApiResult<ICollection<UserChatResponseDto>?>> GetUserChatsAsync(Guid userId);
    }
}
