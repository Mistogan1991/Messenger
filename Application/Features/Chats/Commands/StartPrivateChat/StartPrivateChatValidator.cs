using FluentValidation;

namespace Messenger.Application.Features.Chats.Commands.StartPrivateChat;

public sealed class StartPrivateChatValidator : AbstractValidator<StartPrivateChatCommand>
{
    public StartPrivateChatValidator()
    {
        RuleFor(x => x.OtherUserId)
            .NotEmpty()
            .WithMessage("Other user id is required.");
    }
}
