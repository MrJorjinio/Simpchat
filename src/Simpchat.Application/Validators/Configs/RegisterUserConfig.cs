using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators.Configs
{
    internal static class RegisterUserConfig
    {
        public const int NameMaxLength = 30;
        public const int NameMinLength = 5;
        public const int DescriptionMaxLength = 100;
        public const int DescriptionMinLength = 0;
        public const int PasswordMinLength = 10;
        public const int PasswordMaxLength = 25;
    }
}
