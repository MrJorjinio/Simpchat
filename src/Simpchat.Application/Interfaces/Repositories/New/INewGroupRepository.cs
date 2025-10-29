using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities.Channels;
using Simpchat.Domain.Entities.Groups;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Repositories.New
{
    public interface INewGroupRepository : IBaseRepository<Group>, ISearchableRepository<Group>
    {
        Task AddMemberAsync(Guid userId, Guid groupId);
        Task DeleteMemberAsync(GroupMember groupMember);
        Task<List<Group>> GetUserParticipatedGroupsAsync(Guid userId);
    }
}
