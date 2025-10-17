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
                .Select(cs => new
                {
                    ChannelId = cs.ChannelId,
                    ChatId = cs.Channel.Chat.Id,
                    AvatarUrl = cs.Channel.AvatarUrl,
                    Name = cs.Channel.Name
                })
                .AsNoTracking()
                .ToListAsync();

            if (!metas.Any())
                return new List<UserChatResponseDto>();

            var result = new List<UserChatResponseDto>();

            foreach (var meta in metas)
            {
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
                    .AsNoTracking()
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
                    Id = meta.ChannelId,
                    AvatarUrl = meta.AvatarUrl,
                    Name = meta.Name,
                    Type = ChatType.Channel,
                    LastMessage = lastMessageDto,
                    NotificationsCount = notifCount,
                    UserLastMessage = userLast
                });
            }

            return result
                .OrderByDescending(x => x.UserLastMessage ?? DateTimeOffset.MinValue)
                .ToList();
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
