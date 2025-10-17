using Simpchat.Application.Common.Models.ApiResults;
using Simpchat.Application.Common.Models.Chats.Post;
using Simpchat.Application.Common.Models.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Services
{
    public interface IGroupService
    {
        Task<ApiResult> CreateAsync(Guid userId, ChatPostDto chatPostDto, FileUploadRequest? avatar);
    }
}
