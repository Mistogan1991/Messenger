using Messenger.API.Contracts.Profile;
using Messenger.Application.Features.Users.Privacy.Commands.UpdatePrivacySettings;
using Messenger.Application.Features.Users.Profile.Commands.ConfirmPhoneNumberChange;
using Messenger.Application.Features.Users.Profile.Commands.RequestPhoneNumberChangeOtp;
using Messenger.Application.Features.Users.Profile.Commands.SetUsername;
using Messenger.Application.Features.Users.Profile.Commands.UpdateMyProfile;

namespace Messenger.API.Mappings;

public static class ProfileMappings
{
    public static UpdatePrivacySettingsCommand ToCommand(this UpdatePrivacySettingsRequest request)
    {
        return new UpdatePrivacySettingsCommand(
            request.LastSeen,
            request.PhoneNumber,
            request.ProfilePhoto,
            request.Calls,
            request.ForwardedMessages
        );
    }

    public static SetUsernameCommand ToCommand(this SetUsernameRequest request)
    {
        return new SetUsernameCommand(request.Username);
    }

    public static RequestPhoneNumberChangeOtpCommand ToCommand(this RequestPhoneNumberChangeOtpRequest request)
    {
        return new RequestPhoneNumberChangeOtpCommand(request.PhoneNumber);
    }

    public static ConfirmPhoneNumberChangeCommand ToCommand(this ConfirmPhoneNumberChangeRequest request)
    {
        return new ConfirmPhoneNumberChangeCommand(request.PhoneNumber, request.Code);
    }

    public static UpdateMyProfileCommand ToCommand(this UpdateMyProfileRequest request)
    {
        return new UpdateMyProfileCommand(request.FirstName, request.LastName, request.Bio);
    }
}
