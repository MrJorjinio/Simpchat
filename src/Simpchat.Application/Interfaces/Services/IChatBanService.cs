using Simpchat.Application.Models.ApiResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IChatBanService
    {
        Task<ApiResult<Guid>> BanUserAsync(Guid chatId, Guid userId);
        Task<ApiResult> DeleteAsync(Guid chatId, Guid userId);
    }
}
