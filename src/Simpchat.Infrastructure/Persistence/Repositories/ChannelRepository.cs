using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly SimpchatDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public ChannelRepository(SimpchatDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task<ICollection<UserChatResponseDto>?> GetUserSubscribedChannelsAsync(Guid currentUserId)
        {
            var metas = await _dbContext.ChannelsSubscribers
                .Where(cs => cs.UserId == currentUserId)
                .Select(cs => new {
                    ChannelId = cs.ChannelId,
                    ChatId = cs.Channel.Chat.Id,
                    AvatarUrl = cs.Channel.AvatarUrl,
                    Name = cs.Channel.Name
                })
                .AsNoTracking()
                .ToListAsync();

            var chatIds = metas.Select(m => m.ChatId).Distinct().ToList();
            if (!chatIds.Any()) return new List<UserChatResponseDto>();

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
                    Id = meta.ChannelId,
                    AvatarUrl = meta.AvatarUrl,
                    Name = meta.Name,
                    Type = ChatType.Channel,
                    LastMessage = lastMessageDto,
                    NotificationsCount = notif?.Count ?? 0,
                    UserLastMessage = userLast?.Last
                };
            })
            .OrderByDescending(x => x.UserLastMessage ?? DateTimeOffset.MinValue)
            .ToList();

            return dtos;
        }

        public async Task<ICollection<ChatSearchResponseDto>?> SearchByNameAsync(string searchTerm)
        {
            var channels = await _dbContext.Groups
                .Where(g => EF.Functions.Like(g.Name, $"%{searchTerm}%"))
                .ToListAsync();

            var channelsDtos = channels.Select(g => new ChatSearchResponseDto
            {
                EntityId = g.Id,
                ChatId = g.Id,
                AvatarUrl = g.AvatarUrl,
                DisplayName = g.Name,
                ChatType = ChatType.Group
            }).ToList();

            return channelsDtos;
        }
    }
}
