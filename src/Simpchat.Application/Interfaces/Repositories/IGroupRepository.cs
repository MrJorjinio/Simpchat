using Simpchat.Application.Common.Repository;
using Simpchat.Domain.Entities;

namespace Simpchat.Application.Interfaces.Repositories
{
    public interface IGroupRepository : IBaseRepository<Group>, ISearchableRepository<Group>
    {
        Task AddMemberAsync(Guid userId, Guid groupId);
        Task DeleteMemberAsync(GroupMember groupMember);
        Task<List<Group>> GetUserParticipatedGroupsAsync(Guid userId);
    }
}
