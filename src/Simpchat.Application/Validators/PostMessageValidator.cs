using FluentValidation;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Application.Validators.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators
{
    public class PostMessageValidator : AbstractValidator<PostMessageApiRequestDto>
    {
        public PostMessageValidator()
        {
            RuleFor(m => m.Content)
                .MaximumLength(PostMessageConfig.ContentMaxLength)
                    .WithMessage($"Content max length is {PostMessageConfig.ContentMaxLength}");
        }
    }
}
