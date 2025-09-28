using Microsoft.AspNetCore.Identity;
using SimpchatWeb.Services.Interfaces.Auth;
using System.Security.Cryptography;
using System.Text;

namespace SimpchatWeb.Services.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Encrypt(string password, string salt)
        {
            using var algorithm = new Rfc2898DeriveBytes(
password: password,
salt: Encoding.UTF8.GetBytes(salt),
iterations: 10,
hashAlgorithm: HashAlgorithmName.SHA256);
            return Convert.ToBase64String(algorithm.GetBytes(64));
        }

        public bool Verify(string password, string salt, string passwordHash)
        {
            var requestHash = Encrypt(password, salt);
            if (requestHash != passwordHash)
            {
                return false;
            }
            return true;
        }
    }
}
