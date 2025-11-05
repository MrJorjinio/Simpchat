using FluentValidation;
using Simpchat.Application.Models.Messages;
using Simpchat.Application.Validators.Configs;

public class PostMessageValidator : AbstractValidator<PostMessageDto>
{
    public PostMessageValidator()
    {
        RuleFor(m => m.Content)
            .NotEmpty()
                .WithMessage("Message content cannot be empty")
            .MinimumLength(PostMessageConfig.ContentMinLength)
                .WithMessage($"Message content must be at least {PostMessageConfig.ContentMinLength} character")
            .MaximumLength(PostMessageConfig.ContentMaxLength)
                .WithMessage($"Message content mac length is {PostMessageConfig.ContentMaxLength} characters");
    }
}