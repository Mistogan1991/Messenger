using Messenger.Domain.Enums;

namespace Messenger.Application.Features.Users.Dtos;

public sealed record PrivacySettingsDto(
    PrivacyLevel LastSeen,
    PrivacyLevel PhoneNumber,
    PrivacyLevel ProfilePhoto,
    PrivacyLevel Calls,
    PrivacyLevel ForwardedMessages
);
