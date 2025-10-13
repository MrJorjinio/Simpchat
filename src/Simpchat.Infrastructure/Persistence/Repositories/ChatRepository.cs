using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Interfaces.External.FileStorage;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Domain.Entities;
using Simpchat.Domain.Entities.Chats;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    
    internal class ChatRepository : IChatRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public ChatRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddMessageAsync(Message message)
        {
            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(Chat chat)
        {
            await _dbContext.Chats.AddAsync(chat);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Chat chat)
        {
            _dbContext.Chats.Remove(chat);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Chat?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Chats
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Group)
                .Include(c => c.Channel)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ICollection<Chat>?> GetUserChatsAsync(Guid userId)
        {
            var chats = await _dbContext.ChatsParticipants
                .Include(cp => cp.Chat)
                .Include(cp => cp.Chat.Group)
                .Include(cp => cp.Chat.Channel)
                .Include(cp => cp.Chat.Participants)
                .Include(cp => cp.Chat.Messages)
                .Where(cp => cp.UserId == userId)
                .Select(c => new Chat
                {
                    Id = c.ChatId,
                    Group = c.Chat.Group,
                    Channel = c.Chat.Channel,
                    Participants = c.Chat.Participants,
                    CreatedAt = c.Chat.CreatedAt,
                    Conversation = c.Chat.Conversation,
                    Messages = c.Chat.Messages
                })
                .ToListAsync();

            return chats;
        }

        public async Task<ICollection<Chat>?> SearchByNameAsync(string searchTerm, Guid userId)
        {
            var chats = await _dbContext.Chats
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Group)
                .Include(c => c.Channel)
                .Where(c =>
                    (c.Type == ChatType.Conversation &&
                     c.Participants.Any(p => p.UserId != userId &&
                                             p.User.Username.Contains(searchTerm)))
                    || (c.Type == ChatType.Group &&
                        c.Group != null &&
                        c.Group.Name.Contains(searchTerm))
                    || (c.Type == ChatType.Channel &&
                        c.Channel != null &&
                        c.Channel.Name.Contains(searchTerm))
                )
                .ToListAsync();

            return chats;
        }

        public async Task UpdateAsync(Chat chat)
        {
            _dbContext.Chats.Update(chat);
            await _dbContext.SaveChangesAsync();
        }
    }
}
