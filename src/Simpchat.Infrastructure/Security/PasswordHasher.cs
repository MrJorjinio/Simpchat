using System.Security.Cryptography;
using System.Text;
using Simpchat.Application.Interfaces.Auth;

namespace Simpchat.Infrastructure.Security
{
    internal class PasswordHasher : IPasswordHasher
    {
        private const int SaltSizeInBytes = 16;
        private const int IterationCount = 10000;

        public async Task<string> EncryptAsync(string password, string salt)
        {
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            using var algorithm = new Rfc2898DeriveBytes(
                password: password,
                salt: saltBytes,
                iterations: IterationCount,
                hashAlgorithm: HashAlgorithmName.SHA256);
            return Convert.ToBase64String(algorithm.GetBytes(64));
        }

        public Task<string> GenerateSaltAsync()
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[SaltSizeInBytes];
            rng.GetBytes(saltBytes);
            return Task.FromResult(Convert.ToBase64String(saltBytes));
        }

        public async Task<bool> VerifyAsync(string hash, string password, string salt)
        {
            var requestHash = await EncryptAsync(password, salt);
            return requestHash == hash;
        }
    }
}
