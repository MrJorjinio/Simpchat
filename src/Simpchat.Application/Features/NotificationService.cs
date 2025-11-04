using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;

        public NotificationService(INotificationRepository notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<ApiResult> SetAsSeenAsync(Guid notificationId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);

            if (notification is null)
            {
                return ApiResult.FailureResult($"Notification with ID[{notificationId}] not found");
            }

            notification.IsSeen = true;

            await _notificationRepo.UpdateAsync(notification);

            return ApiResult.SuccessResult();
        }
    }
}
