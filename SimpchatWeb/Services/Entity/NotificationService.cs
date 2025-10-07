using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpchatWeb.Services.Db.Contexts.Default;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using SimpchatWeb.Services.Db.Contexts.Default.Models.ChatMessageDtos.Responses;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserNotificationDtos.Responses;
using SimpchatWeb.Services.Interfaces.Auth;
using SimpchatWeb.Services.Interfaces.Entity;

namespace SimpchatWeb.Services.Entity
{
    public class NotificationService : INotificationService
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SimpchatDbContext _dbContext;
        public NotificationService(
            ITokenService tokenService,
            IMapper mapper,
            SimpchatDbContext dbContext
            )
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public async Task<ApiResult> DeleteNotificationAsync(User user, Guid messageId)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.ReceiverId == user.Id && n.MessageId == messageId);

            if (notification is null)
                return new ApiResult(false, 404, "Notification not found");

            _dbContext.Notifications.Remove(notification);
            await _dbContext.SaveChangesAsync();

            return new ApiResult(true, 200, "Success");
        }

        public async Task<ApiResult<ICollection<UserNotificationGetResponseDto>>> GetMyNotificationsAsync(User user)
        {
            var userNotifications = await _dbContext.Notifications
                .Where(un => un.ReceiverId == user.Id && un.IsSeen == false)
                .Include(un => un.Message)
                    .ThenInclude(m => m.Chat)
                        .ThenInclude(c => c.Channel)
                .Include(c => c.Message.Chat.Participants)
                    .ThenInclude(p => p.User)
                .Include(s => s.Message.Sender)
                .Include(c => c.Message.Chat.Group)
                .OrderByDescending(un => un.Message.SentAt)
                .ToListAsync();

            var response = userNotifications.Select(un => new UserNotificationGetResponseDto
            {
                ChatId = un.Message.ChatId,
                MessageId = un.Message.Id,
                ChatName = un.Message.Chat.Type == ChatType.Channel
                ? un.Message.Chat.Channel.Name
                : un.Message.Chat.Type == ChatType.Conversation
                    ? un.Message.Chat.Participants
                        .FirstOrDefault(p => p.UserId != user.Id)?.User.Username
                    : un.Message.Chat.Type == ChatType.Group
                        ? un.Message.Chat.Group.Name
                : null,
                Content = un.Message.Content,
                SenderName = un.Message.Sender.Username,
                SentAt = un.Message.SentAt,
                ChatType = un.Message.Chat.Type
            }).ToList();

            return new ApiResult<ICollection<UserNotificationGetResponseDto>>(true, 200, "Success", response);
        }

        public async Task<ApiResult<ChatMessageGetByIdGetResponseDto>> MarkAsSeenAsync(User user, Guid messageId)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.MessageId == messageId && n.ReceiverId == user.Id);

            if (notification == null)
                return new ApiResult<ChatMessageGetByIdGetResponseDto>(false, 404, "Notification not found");

            notification.IsSeen = true;
            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<ChatMessageGetByIdGetResponseDto>(notification);
            return new ApiResult<ChatMessageGetByIdGetResponseDto>(true, 200, "Success", response);
        }
    }
}
