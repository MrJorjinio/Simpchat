using FluentValidation;
using Simpchat.Application.Models.Users;
using Simpchat.Application.Validators.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators
{
    public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordDto>
    {
        public UpdatePasswordValidator()
        {
            RuleFor(u => u.NewPassword)
                .NotEmpty().WithMessage("New Password is required.")
                .MaximumLength(RegisterUserConfig.PasswordMaxLength)
                    .WithMessage($"New Password max length is {UpdatePasswordConfig.PasswordMaxLength}")
                .MinimumLength(RegisterUserConfig.PasswordMinLength)
                    .WithMessage($"New Password min length is {UpdatePasswordConfig.PasswordMinLength}");
            RuleFor(u => u.CurrentPassword)
                .NotEmpty().WithMessage("Current Password is required.")
                .MaximumLength(RegisterUserConfig.PasswordMaxLength)
                    .WithMessage($"Current Password max length is {UpdatePasswordConfig.PasswordMaxLength}")
                .MinimumLength(RegisterUserConfig.PasswordMinLength)
                    .WithMessage($"Current Password min length is {UpdatePasswordConfig.PasswordMinLength}");
        }
    }
}
