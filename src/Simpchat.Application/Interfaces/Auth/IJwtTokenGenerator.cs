using Simpchat.Domain.Entities;

namespace Simpchat.Application.Interfaces.Auth
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateJwtTokenAsync(Guid userId, IEnumerable<GlobalRole> roles);
    }
}
