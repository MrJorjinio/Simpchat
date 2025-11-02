using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators.Configs;

public static class UpdateUserInfoConfig
{
    public const int UsernameMinLength = 3;
    public const int UsernameMaxLength = 50;
    public const int DescriptionMaxLength = 200;
}
