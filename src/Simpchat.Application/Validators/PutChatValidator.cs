using FluentValidation;
using Simpchat.Application.Models.Chats;
using Simpchat.Application.Validators.Configs;

namespace Simpchat.Application.Validators
{
    public class PutChatValidator : AbstractValidator<PutChatDto>
    {
        public PutChatValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                    .WithMessage("Chat name cannot be empty")
                .MinimumLength(ChatConfig.ChatNameMinLength)
                    .WithMessage($"Chat name must be at least {ChatConfig.ChatNameMinLength} character")
                .MaximumLength(ChatConfig.ChatNameMaxLength)
                    .WithMessage($"Chat name cannot exceed {ChatConfig.ChatNameMaxLength} characters");

            RuleFor(c => c.Description)
                .MaximumLength(ChatConfig.ChatDescriptionMaxLength)
                    .WithMessage($"Chat description cannot exceed {ChatConfig.ChatDescriptionMaxLength} characters");

            When(c => c.Avatar != null, () =>
            {
                RuleFor(c => c.Avatar!.FileName)
                    .NotEmpty()
                        .WithMessage("Avatar filename cannot be empty")
                    .MinimumLength(ChatConfig.AvatarFileNameMinLength)
                        .WithMessage($"Avatar filename must be at least {ChatConfig.AvatarFileNameMinLength} character");
            });
        }
    }
}
