using System.Security.Claims;

namespace SimpchatWeb.Services.Interfaces.Token
{
    public interface ITokenService
    {
        Guid GetUserId(
            ClaimsPrincipal user
            );
    }
}
