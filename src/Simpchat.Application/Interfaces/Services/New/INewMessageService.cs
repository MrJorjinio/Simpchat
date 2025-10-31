using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Chats.Post.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services.New
{
    public interface INewMessageService
    {
        Task<ApiResult> AddMessage(PostMessageDto postMessageDto);
    }
}
