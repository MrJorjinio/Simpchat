using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserNotificationDtos.Responses;
using SimpchatWeb.Services.Entity;

namespace SimpchatWeb.Services.Interfaces.Entity
{
    public interface INotificationService
    {
        Task<ApiResult> DeleteNotificationAsync(User user, Guid messageId);
        Task<ApiResult<ChatMessageGetByIdGetResponseDto>> MarkAsSeenAsync(User user, Guid messageId);
        Task<ApiResult<ICollection<UserNotificationGetResponseDto>>> GetMyNotificationsAsync(User user);
    }
}
