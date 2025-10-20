using Simpchat.Application.Common.Models.Chats.Get.ById;
using Simpchat.Application.Common.Models.Chats.Get.Profile;
using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Application.Common.Models.Chats.Post.Message;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task<ICollection<ChatSearchResponseDto>?> SearchByNameAsync(string searchTerm, Guid userId);
        Task<ICollection<UserChatResponseDto>?> GetUserChatsAsync(Guid userId);
        Task<ChatGetByIdDto> GetByIdAsync(Guid chatId, Guid userId);
        Task<Chat> GetByIdAsync(Guid chatId);
        Task<ChatPermission> GetPermissionByNameAsync(string name);
        Task<ChatGetByIdProfile> GetProfileByIdAsync(Guid chatId, Guid userId);
        Task<Message> AddMessageAsync(MessagePostDto message, User currentUser);
        Task UpdateAsync(Chat chat);
        Task<Chat> CreateAsync(Chat chat);
    }
}
