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
    public class PostReactionValidator : AbstractValidator<PostReactionDto>
    {
        public PostReactionValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                    .WithMessage($"Name is required")
                .MaximumLength(PostReactionConfig.NameMaxLength)
                    .WithMessage($"Name max length is {PostReactionConfig.NameMaxLength}")
                .MinimumLength(PostReactionConfig.NameMinLength)
                    .WithMessage($"Name max length is {PostReactionConfig.NameMinLength}");
        }
    }
}
