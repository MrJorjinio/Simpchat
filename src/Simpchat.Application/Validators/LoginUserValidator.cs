using FluentValidation;
using Simpchat.Application.Models.Users;
using Simpchat.Application.Validators.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators
{
    internal class LoginUserValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Credential)
                .NotEmpty().WithMessage("Credential is required.")
                .Must(IsEmailOrUsername)
                .WithMessage($"Credential must be a valid email or username ({LoginUserConfig.CredentialMinLength}-{LoginUserConfig.CredentialMaxLength} characters, allowed: letters, digits, ., _, -).");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(LoginUserConfig.PasswordMinLength)
                    .WithMessage($"Password min length is {LoginUserConfig.PasswordMinLength}")
                .MaximumLength(LoginUserConfig.PasswordMaxLength)
                    .WithMessage($"Password max length is {LoginUserConfig.PasswordMaxLength}");
        }

        private static bool IsEmailOrUsername(string credential)
        {
            if (string.IsNullOrWhiteSpace(credential))
                return false;

            if (Regex.IsMatch(credential, LoginUserConfig.EmailRegex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                return true;

            if (credential.Length < LoginUserConfig.CredentialMinLength || credential.Length > LoginUserConfig.CredentialMaxLength)
                return false;

            return Regex.IsMatch(credential, LoginUserConfig.UsernameRegex);
        }
    }
}
