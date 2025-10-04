using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts;
using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Responses;

namespace SimpchatWeb.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        bool Register(
            UserRegisterPostDto user
            );
        string Login(
            UserLoginPostDto user
            );
    }
}
