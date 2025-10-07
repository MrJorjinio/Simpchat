using SimpchatWeb.Services.Interfaces.Auth;
using System.Security.Claims;

namespace SimpchatWeb.Services.Auth
{
    public class TokenService : ITokenService
    {
        public Guid GetUserId(
            ClaimsPrincipal user
            )
        {
            if (user is null)
            {
                return Guid.Empty;
            }

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id is null)
            {
                return Guid.Empty;
            }
            if (string.IsNullOrWhiteSpace(id))
                return Guid.Empty;

            return Guid.TryParse(id!, out Guid guid) ? guid : Guid.Empty;
        }
    }
}
