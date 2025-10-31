using Simpchat.Application.Interfaces.Repositories.New;
using Simpchat.Application.Interfaces.Services.New;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Models.Chats.Get.ById;
using Simpchat.Application.Models.Chats.Get.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Features.New
{
    internal class NewChatService : INewChatService
    {
        private readonly INewChatRepository _repo;

        public NewChatService(INewChatRepository chatRepository)
        {
            _repo = chatRepository;
        }

        public async Task<GetByIdChatDto> GetByIdAsync(Guid chatId)
        {
            var chat = await _repo.GetByIdAsync(chatId);
        }

        public Task<GetByIdChatProfile> GetProfileAsync(Guid chatId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid chatId, PutChatDto postChatDto)
        {
            throw new NotImplementedException();
        }
    }
}
