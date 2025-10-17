using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;
using SimpchatWeb.Services.Entity;

namespace SimpchatWeb.Services.Interfaces.Entity
{
    public interface IChatService
    {
        Task<ApiResult<ChatGetByIdGetResponseDto>> GetChatByIdAsync(Guid chatId, User currentUser);
        Task<ApiResult<ChatConversationGetByIdGetResponse>> GetConversationByIdAsync(Guid chatId, User currentUser);
        Task<ApiResult<IEnumerable<ChatSearchGetResponseDto>>> SearchChatsAsync(string name);
        Task<ApiResult<UserJoinChatResponseDto>> JoinChatAsync(User currentUser, Chat chat);
        Task<ApiResult<ChatGetByIdGetResponseDto>> UpdatePrivacyTypeAsync(Chat chat, ChatPrivacyType chatPrivacyType);
        Task<ApiResult<UserJoinChatResponseDto>> AddUserToChatAsync(User fromUser, Guid chatId, Guid joiningUserId);
        Task<ApiResult<IEnumerable<GetMyChatGetResponseDto>>> GetMyChatsAsync(User currentUser);
        Task<ApiResult<ChatMessageGetByIdGetResponseDto>> SendMessageAsync(User currentUser, Guid? chatId, ChatMessagePostDto model);
    }
}
