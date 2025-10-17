using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;
using System.Threading.Tasks;

namespace SimpchatWeb.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(UserRegisterPostDto user);
        Task<string> LoginAsync(UserLoginPostDto user);
    }
}
