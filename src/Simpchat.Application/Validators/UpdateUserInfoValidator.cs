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
    public class UpdateUserInfoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserInfoValidator()
        {
            RuleFor(u => u.Username)
                 .NotEmpty()
                     .WithMessage("Username cannot be empty")
                 .MinimumLength(UpdateUserInfoConfig.UsernameMinLength)
                     .WithMessage($"Username must be at least {UpdateUserInfoConfig.UsernameMinLength} characters long")
                 .MaximumLength(UpdateUserInfoConfig.UsernameMaxLength)
                     .WithMessage($"Username cannot exceed {UpdateUserInfoConfig.UsernameMaxLength} characters");

            RuleFor(u => u.Description)
                .MaximumLength(UpdateUserInfoConfig.DescriptionMaxLength)
                    .WithMessage($"Description cannot exceed {UpdateUserInfoConfig.DescriptionMaxLength} characters");
        }
    }
}
