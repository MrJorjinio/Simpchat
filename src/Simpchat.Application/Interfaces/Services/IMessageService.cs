using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IMessageService
    {
        Task<ApiResult<Guid>> SendMessageAsync(PostMessageDto postMessageDto);
    }
}
