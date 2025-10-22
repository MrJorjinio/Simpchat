using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<ApiResult> SetSeenAsync(Guid messageId, Guid userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(messageId, userId);

            if (notification is null)
            {
                return ApiResult.FailureResult($"Notification with MessageId[{messageId}] and UserId[{userId}] not found");
            }

            notification.IsSeen = true;
            await _notificationRepository.UpdateAsync(notification);

            return ApiResult.SuccessResult();
        }
    }
}
