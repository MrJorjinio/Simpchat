using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators.Configs
{
    public static class ChatConfig
    {
        public const int ChatNameMinLength = 1;
        public const int ChatNameMaxLength = 100;
        public const int ChatDescriptionMaxLength = 500;
        public const int AvatarFileNameMinLength = 1;
    }
}
