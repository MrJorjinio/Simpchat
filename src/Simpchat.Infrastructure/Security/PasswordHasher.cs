using Microsoft.AspNetCore.Identity;
using Simpchat.Application.Interfaces.Auth;
using Simpchat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.Security
{
    internal class PasswordHasher : IPasswordHasher
    {
        public async Task<string> EncryptAsync(string password, string salt)
        {
            using var algorithm = new Rfc2898DeriveBytes(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                iterations: 10,
                hashAlgorithm: HashAlgorithmName.SHA256);
            return Convert.ToBase64String(algorithm.GetBytes(64));
        }

        public async Task<bool> VerifyAsync(string hash, string password, string salt)
        {
            var requestHash = await EncryptAsync(password, salt);
            if (requestHash != hash)
            {
                return false;
            }
            return true;
        }
    }
}
