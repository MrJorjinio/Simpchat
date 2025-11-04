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
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {

        public ResetPasswordValidator()
        {
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(RegisterUserConfig.PasswordMaxLength)
                    .WithMessage($"Password max length is {ResetPasswordConfig.PasswordMaxLength}")
                .MinimumLength(RegisterUserConfig.PasswordMinLength)
                    .WithMessage($"Password min length is {ResetPasswordConfig.PasswordMinLength}");
        }
    }
}
