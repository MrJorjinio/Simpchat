using System.Security.Claims;

namespace SimpchatWeb.Services.Interfaces.Auth
{
    public interface ITokenService
    {
        Guid GetUserId(
            ClaimsPrincipal user
            );
    }
}
