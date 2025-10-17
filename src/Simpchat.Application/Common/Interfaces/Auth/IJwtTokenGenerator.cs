using Simpchat.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.Auth
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateJwtTokenAsync(Guid userId, IEnumerable<GlobalRole> roles);
    }
}
