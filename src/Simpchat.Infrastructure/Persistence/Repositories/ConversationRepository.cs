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
                .Select(c => new {
                    ConversationId = c.Id,
                    ChatId = c.Chat.Id,
                    OtherUserId = c.UserId1 == currentUserId ? c.UserId2 : c.UserId1
                })
                .AsNoTracking()
                .ToListAsync();

            if (!metas.Any())
                return new List<UserChatResponseDto>();

            var otherUserIds = metas.Select(m => m.OtherUserId).Distinct().ToList();
            var otherUsers = await _dbContext.Users
                .Where(u => otherUserIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Username, u.AvatarUrl })
                .AsNoTracking()
                .ToListAsync();

            var userDict = otherUsers.ToDictionary(u => u.Id);

            var chatIds = metas.Select(m => m.ChatId).ToList();

            var lastMessages = await _dbContext.Messages
                .Where(m => chatIds.Contains(m.ChatId))
                .GroupBy(m => m.ChatId)
                .Select(g => g.OrderByDescending(x => x.SentAt).ThenByDescending(x => x.Id).FirstOrDefault())
                .Where(m => m != null)
                .Select(m => new {
                    m.ChatId,
                    MessageId = m.Id,
                    m.Content,
                    m.FileUrl,
                    SenderUsername = m.Sender.Username,
                    m.SentAt
                })
                .AsNoTracking()
                .ToListAsync();

            var notifications = await _dbContext.Notifications
                .Where(n => chatIds.Contains(n.Message.ChatId) && n.ReceiverId == currentUserId && !n.IsSeen)
                .GroupBy(n => n.Message.ChatId)
                .Select(g => new { ChatId = g.Key, Count = g.Count() })
                .ToListAsync();

            var userLasts = await _dbContext.Messages
                .Where(m => chatIds.Contains(m.ChatId) && m.SenderId == currentUserId)
                .GroupBy(m => m.ChatId)
                .Select(g => new { ChatId = g.Key, Last = (DateTimeOffset?)g.Max(m => m.SentAt) })
                .ToListAsync();

            var dtos = metas.Select(meta =>
            {
                var otherUser = userDict.TryGetValue(meta.OtherUserId, out var u) ? u : null;
                var lm = lastMessages.FirstOrDefault(x => x.ChatId == meta.ChatId);
                var notif = notifications.FirstOrDefault(x => x.ChatId == meta.ChatId);
                var userLast = userLasts.FirstOrDefault(x => x.ChatId == meta.ChatId);

                var lastMessageDto = lm == null ? null : new LastMessageResponseDto
                {
                    Content = lm.Content,
                    FileUrl = lm.FileUrl,
                    SenderUsername = lm.SenderUsername,
                    SentAt = lm.SentAt,
                };

                return new UserChatResponseDto
                {
                    Id = meta.ConversationId,
                    AvatarUrl = otherUser?.AvatarUrl,
                    Name = otherUser?.Username,
                    Type = ChatType.Conversation,
                    LastMessage = lastMessageDto,
                    NotificationsCount = notif?.Count ?? 0,
                    UserLastMessage = userLast?.Last
                };
            })
            .OrderByDescending(x => x.UserLastMessage ?? DateTimeOffset.MinValue)
            .ToList();

            return dtos;
        }
    }
}
