using Messenger.Domain.Common;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Aggregates.Users;

public sealed class UserPrivacySettings : ValueObject
{
    public PrivacyLevel LastSeen { get; }
    public PrivacyLevel PhoneNumber { get; }
    public PrivacyLevel ProfilePhoto { get; }
    public PrivacyLevel Calls { get; }
    public PrivacyLevel ForwardedMessages { get; }

    private UserPrivacySettings()
    {
    }

    private UserPrivacySettings(
        PrivacyLevel lastSeen,
        PrivacyLevel phoneNumber,
        PrivacyLevel profilePhoto,
        PrivacyLevel calls,
        PrivacyLevel forwardedMessages)
    {
        LastSeen = lastSeen;
        PhoneNumber = phoneNumber;
        ProfilePhoto = profilePhoto;
        Calls = calls;
        ForwardedMessages = forwardedMessages;
    }

    public static UserPrivacySettings Default()
    {
        return new UserPrivacySettings(
            PrivacyLevel.Everybody,
            PrivacyLevel.MyContacts,
            PrivacyLevel.Everybody,
            PrivacyLevel.MyContacts,
            PrivacyLevel.Everybody);
    }

    public UserPrivacySettings Update(
        PrivacyLevel lastSeen,
        PrivacyLevel phoneNumber,
        PrivacyLevel profilePhoto,
        PrivacyLevel calls,
        PrivacyLevel forwardedMessages)
    {
        return new UserPrivacySettings(
            lastSeen,
            phoneNumber,
            profilePhoto,
            calls,
            forwardedMessages);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return LastSeen;
        yield return PhoneNumber;
        yield return ProfilePhoto;
        yield return Calls;
        yield return ForwardedMessages;
    }
}