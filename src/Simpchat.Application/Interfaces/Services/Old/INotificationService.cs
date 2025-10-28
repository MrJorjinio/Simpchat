using Simpchat.Application.Models.ApiResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services.Old
{
    public interface INotificationService
    {
        Task<ApiResult> SetSeenAsync(Guid messageId, Guid userId);
    }
}
