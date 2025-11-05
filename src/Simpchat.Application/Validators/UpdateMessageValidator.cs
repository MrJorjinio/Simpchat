using FluentValidation;
using Simpchat.Application.Models.Messages;
using Simpchat.Application.Validators.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Validators
{
    public class UpdateMessageValidator : AbstractValidator<UpdateMessageDto>
    {
        public UpdateMessageValidator()
        {
            RuleFor(m => m.Content)
            .NotEmpty()
                .WithMessage("Message content cannot be empty")
            .MinimumLength(UpdateMessageConfig.ContentMinLength)
                .WithMessage($"Message content must be at least {UpdateMessageConfig.ContentMinLength} character")
            .MaximumLength(UpdateMessageConfig.ContentMaxLength)
                .WithMessage($"Message content mac length is {UpdateMessageConfig.ContentMaxLength} characters");
        }
    }
}
