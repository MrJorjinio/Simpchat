namespace SimpchatWeb.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string Encrypt(string password, string salt);
        bool Verify(string password, string salt, string passwordHash);
    }
}
