using FluentValidation;
using Simpchat.Application.Models.Reactions;
using Simpchat.Application.Validators.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators
{
    public class UpdateReactionValidator : AbstractValidator<UpdateReactionDto>
    {
        public UpdateReactionValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                    .WithMessage($"Name is required")
                .MaximumLength(PostReactionConfig.NameMaxLength)
                    .WithMessage($"Name max length is {UpdateReactionConfig.NameMaxLength}")
                .MinimumLength(PostReactionConfig.NameMinLength)
                    .WithMessage($"Name max length is {UpdateReactionConfig.NameMinLength}");
        }
    }
}
