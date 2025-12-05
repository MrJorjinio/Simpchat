using Simpchat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        Task<string> EncryptAsync(string password, string salt);
        Task<bool> VerifyAsync(string hash, string password, string salt);
        Task<string> GenerateSaltAsync();
    }
}
