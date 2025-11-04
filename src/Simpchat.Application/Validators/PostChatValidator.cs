using FluentValidation;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Validators.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators
{
    public class PostChatValidator : AbstractValidator<PostChatDto>
    {
        public PostChatValidator()
        {
            RuleFor(c => c.Name)
                .MaximumLength(PostChatConfig.NameMaxLength)
                    .WithMessage($"Name max length is {PostChatConfig.NameMaxLength}")
                .MinimumLength(PostChatConfig.NameMinLength)
                    .WithMessage($"Name min length is {PostChatConfig.NameMinLength}");
            RuleFor(c => c.Description)
                .MaximumLength(PostChatConfig.DescriptionMaxLength)
                    .WithMessage($"Description max length is {PostChatConfig.DescriptionMaxLength}")
                .MinimumLength(PostChatConfig.DescriptionMinLength)
                    .WithMessage($"Description min length is {PostChatConfig.DescriptionMinLength}");
        }
    }
}
