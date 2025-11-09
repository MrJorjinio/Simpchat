using Simpchat.Application.Errors;
using Simpchat.Application.Interfaces.Repositories;
using Simpchat.Application.Interfaces.Services;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Shared.Models;
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

        public async Task<Result> SetAsSeenAsync(Guid notificationId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);

            if (notification is null)
            {
                return Result.Failure(ApplicationErrors.Notification.IdNotFound);
            }

            notification.IsSeen = true;

            await _notificationRepo.UpdateAsync(notification);

            return Result.Success();
        }
    }
}
