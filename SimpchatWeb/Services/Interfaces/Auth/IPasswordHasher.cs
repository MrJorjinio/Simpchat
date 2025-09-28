namespace SimpchatWeb.Services.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        string Encrypt(string password, string salt);
        bool Verify(string password, string salt, string passwordHash);
    }
}
