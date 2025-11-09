
using Simpchat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IChatBanService
    {
        Task<Result<Guid>> BanUserAsync(Guid chatId, Guid userId);
        Task<Result> DeleteAsync(Guid chatId, Guid userId);
    }
}
