using Messenger.Domain.Enums;

namespace Messenger.API.Contracts.Profile;

public sealed record UpdatePrivacySettingsRequest(
    PrivacyLevel LastSeen,
    PrivacyLevel PhoneNumber,
    PrivacyLevel ProfilePhoto,
    PrivacyLevel Calls,
    PrivacyLevel ForwardedMessages
);
