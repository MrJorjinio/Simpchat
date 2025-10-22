using Simpchat.Application.Models.Chats.Get.ById;
using Simpchat.Application.Models.Chats.Get.Profile;
using Simpchat.Application.Models.Chats.Get.UserChat;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Application.Models.Chats.Search;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task<ICollection<SearchChatResponseDto>?> SearchByNameAsync(string searchTerm, Guid userId);
        Task<ICollection<UserChatResponseDto>?> GetUserChatsAsync(Guid userId);
        Task<GetByIdChatDto> GetByIdAsync(Guid chatId, Guid userId);
        Task<Chat> GetByIdAsync(Guid chatId);
        Task<ChatPermission> GetPermissionByNameAsync(string name);
        Task<GetByIdChatProfile> GetProfileByIdAsync(Guid chatId, Guid userId);
        Task UpdateAsync(Chat chat);
        Task<Chat> CreateAsync(Chat chat);
    }
}
