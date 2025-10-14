using Microsoft.EntityFrameworkCore;
using Simpchat.Application.Common.Interfaces.Repositories;
using Simpchat.Application.Common.Models.Chats.Search;
using Simpchat.Domain.Entities;
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
