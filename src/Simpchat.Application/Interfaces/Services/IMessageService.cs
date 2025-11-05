using Simpchat.Application.Models.ApiResult;
using Simpchat.Application.Models.ApiResults;
using Simpchat.Application.Models.Files;
using Simpchat.Application.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services
{
    public interface IMessageService
    {
        Task<ApiResult<Guid>> SendMessageAsync(PostMessageDto postMessageDto, UploadFileRequest? uploadFileRequest);
        Task<ApiResult> UpdateAsync(Guid messageId, UpdateMessageDto updateMessageDto, UploadFileRequest? uploadFileRequest);
        Task<ApiResult> DeleteAsync(Guid messageId);
    }
}
