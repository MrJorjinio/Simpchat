using FluentValidation;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Validators.Configs;

public class PostMessageValidator : AbstractValidator<PostMessageApiRequestDto>
{
    public PostMessageValidator()
    {
        RuleFor(m => m.Content)
            .NotEmpty()
                .WithMessage("Message content cannot be empty")
            .MinimumLength(PostMessageApiRequestConfig.ContentMinLength)
                .WithMessage($"Message content must be at least {PostMessageApiRequestConfig.ContentMinLength} character");
    }
}