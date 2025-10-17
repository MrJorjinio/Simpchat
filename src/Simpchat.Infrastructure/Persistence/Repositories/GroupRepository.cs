using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Models.Chats.Get.UserChat;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Domain.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Persistence.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SimpchatDbContext _dbContext;

        public GroupRepository(SimpchatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(Group group)
        {
            await _dbContext.Groups.AddAsync(group);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<UserChatResponseDto>?> GetUserParticipatedGroupsAsync(Guid currentUserId)
        {
            var metas = await _dbContext.GroupsMembers
                .Where(gm => gm.UserId == currentUserId)
                .Select(gm => new
                {
                    GroupId = gm.GroupId,
                    ChatId = gm.Group.Chat.Id,
                    AvatarUrl = gm.Group.AvatarUrl,
                    Name = gm.Group.Name
                })
                .AsNoTracking()
                .ToListAsync();

            var chatIds = metas.Select(m => m.ChatId).ToList();
            if (!chatIds.Any())
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
                    Id = meta.GroupId,
                    AvatarUrl = meta.AvatarUrl,
                    Name = meta.Name,
                    Type = ChatType.Group,
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
            var groups = await _dbContext.Groups
                .Where(g => EF.Functions.Like(g.Name, $"%{searchTerm}%"))
                .ToListAsync();

            var groupsDtos = groups.Select(g => new ChatSearchResponseDto
            {
                EntityId = g.Id,
                ChatId = g.Id,
                AvatarUrl = g.AvatarUrl,
                DisplayName = g.Name,
                ChatType = ChatType.Group
            }).ToList();

            return groupsDtos;
        }
    }
}
