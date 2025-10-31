using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Chats.Get.ById;
using Simpchat.Application.Models.Chats.Get.Profile;
using Simpchat.Application.Models.Chats.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Services.New
{
    public interface INewChatService
    {
        Task UpdateAsync(Guid chatId, PutChatDto postChatDto);
        Task<GetByIdChatProfile> GetProfileAsync(Guid chatId);
        Task<GetByIdChatDto> GetByIdAsync(Guid chatId);
    }
}
