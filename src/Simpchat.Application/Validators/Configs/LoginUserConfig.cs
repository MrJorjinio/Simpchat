using FluentValidation;
using Simpchat.Application.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators.Configs
{
    public static class LoginUserConfig
    {
        public const int CredentialMinLength = RegisterUserConfig.NameMinLength;
        public const int CredentialMaxLength = RegisterUserConfig.NameMaxLength;

        public const int PasswordMinLength = RegisterUserConfig.PasswordMinLength;
        public const int PasswordMaxLength = RegisterUserConfig.PasswordMaxLength;

        public const string EmailRegex = @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$";
        public const string UsernameRegex = @"^[A-Za-z0-9._\-]+$";
    }
}
