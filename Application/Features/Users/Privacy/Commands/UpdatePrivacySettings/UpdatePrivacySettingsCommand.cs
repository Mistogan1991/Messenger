using Messenger.Application.Common.CQRS;
using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Users.Privacy.Commands.UpdatePrivacySettings
{
    public sealed record UpdatePrivacySettingsCommand(
        PrivacyLevel LastSeen,
        PrivacyLevel PhoneNumber,
        PrivacyLevel ProfilePhoto,
        PrivacyLevel Calls,
        PrivacyLevel ForwardedMessages
    ) : IAppRequest;
}
