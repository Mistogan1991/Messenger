using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Profile.Commands.ConfirmPhoneNumberChange;

public sealed record ConfirmPhoneNumberChangeCommand(
    string PhoneNumber,
    string Code
) : IAppRequest;
