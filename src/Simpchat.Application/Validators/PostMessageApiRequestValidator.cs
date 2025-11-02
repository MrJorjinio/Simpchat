using FluentValidation;
using Simpchat.Application.Models.Chats.Post.Message;
using Simpchat.Application.Validators.Configs;

public class PostMessageApiRequestValidator : AbstractValidator<PostMessageApiRequestDto>
{
    public PostMessageApiRequestValidator()
    {
        RuleFor(m => m.Content)
            .NotEmpty()
                .WithMessage("Message content cannot be empty")
            .MinimumLength(PostMessageApiRequestConfig.ContentMinLength)
                .WithMessage($"Message content must be at least {PostMessageApiRequestConfig.ContentMinLength} character");
    }
}