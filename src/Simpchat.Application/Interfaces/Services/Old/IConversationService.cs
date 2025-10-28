using Simpchat.Application.Models.ApiResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services.Old
{
    public interface IConversationService
    {
        Task<ApiResult> DeleteAsync(Guid userId1, Guid userId2);
    }
}
