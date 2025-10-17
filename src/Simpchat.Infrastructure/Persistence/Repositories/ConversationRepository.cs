using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    internal class ConversationRepository : IConversationRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public ConversationRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ICollection<UserChatResponseDto>?> GetUserConversationsAsync(Guid currentUserId)
        {
            var metas = await _dbContext.Conversations
                .Where(c => c.UserId1 == currentUserId || c.UserId2 == currentUserId)
                .Select(c => new
                {
                    ConversationId = c.Id,
                    ChatId = c.Chat.Id,
                    OtherUserId = c.UserId1 == currentUserId ? c.UserId2 : c.UserId1
                })
                .AsNoTracking()
                .ToListAsync();

            if (!metas.Any())
                return new List<UserChatResponseDto>();

            var result = new List<UserChatResponseDto>();

            foreach (var meta in metas)
            {
                var otherUser = await _dbContext.Users
                    .Where(u => u.Id == meta.OtherUserId)
                    .Select(u => new { u.Username, u.AvatarUrl })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                var lastMsg = await _dbContext.Messages
                    .Where(m => m.ChatId == meta.ChatId)
                    .OrderByDescending(m => m.SentAt)
                    .ThenByDescending(m => m.Id)
                    .Select(m => new
                    {
                        m.Id,
                        m.Content,
                        m.FileUrl,
                        SenderUsername = m.Sender.Username,
                        m.SentAt
                    })
                    .FirstOrDefaultAsync();

                var notifCount = await _dbContext.Notifications
                    .CountAsync(n => n.Message.ChatId == meta.ChatId
                                     && n.ReceiverId == currentUserId
                                     && !n.IsSeen);

                var userLast = await _dbContext.Messages
                    .Where(m => m.ChatId == meta.ChatId && m.SenderId == currentUserId)
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => (DateTimeOffset?)m.SentAt)
                    .FirstOrDefaultAsync();

                var lastMessageDto = lastMsg == null ? null : new LastMessageResponseDto
                {
                    Content = lastMsg.Content,
                    FileUrl = lastMsg.FileUrl,
                    SenderUsername = lastMsg.SenderUsername,
                    SentAt = lastMsg.SentAt
                };

                result.Add(new UserChatResponseDto
                {
                    Id = meta.ConversationId,
                    AvatarUrl = otherUser?.AvatarUrl,
                    Name = otherUser?.Username,
                    Type = ChatType.Conversation,
                    LastMessage = lastMessageDto,
                    NotificationsCount = notifCount,
                    UserLastMessage = userLast
                });
            }

            return result
                .OrderByDescending(x => x.UserLastMessage ?? DateTimeOffset.MinValue)
                .ToList();
        }
    }
}
