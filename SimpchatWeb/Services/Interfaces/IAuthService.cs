using SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos;

namespace SimpchatWeb.Services.Interfaces
{
    public interface IAuthService
    {
        UserResponseDto Register(UserRegisterDto user);
        string Login(UserLoginDto user);
    }
}
