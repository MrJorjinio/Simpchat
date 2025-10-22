using FluentValidation;
using Simpchat.Application.Models.Users.Post;
using Simpchat.Application.Validators.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators
{
    internal class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(u => u.Username)
                .MaximumLength(RegisterUserConfig.NameMaxLength)
                    .WithMessage($"Username max length is {RegisterUserConfig.NameMaxLength}")
                .MinimumLength(RegisterUserConfig.NameMinLength)
                    .WithMessage($"Username min length us {RegisterUserConfig.NameMinLength}");
            RuleFor(u => u.Description)
                .MaximumLength(RegisterUserConfig.DescriptionMaxLength)
                    .WithMessage($"Description max lenght is {RegisterUserConfig.DescriptionMaxLength}")
                .MinimumLength(RegisterUserConfig.DescriptionMinLength)
                    .WithMessage($"Description min length us {RegisterUserConfig.DescriptionMinLength}");
            RuleFor(u => u.Password)
                .MaximumLength(RegisterUserConfig.PasswordMaxLength)
                    .WithMessage($"Password max lenght is {RegisterUserConfig.PasswordMaxLength}")
                .MinimumLength(RegisterUserConfig.PasswordMinLength)
                    .WithMessage($"Password min length is {RegisterUserConfig.PasswordMinLength}");
        }
    }
}
