using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories.Old
{
    public interface IMessageRepository
    {
        Task<Message> AddMessageAsync(PostMessageDto message, User currentUser);
        Task UpdateAsync(Message message);
    }
}
