using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Profile.Commands.RequestPhoneNumberChangeOtp;

public sealed record RequestPhoneNumberChangeOtpCommand(
    string PhoneNumber
) : IAppRequest<RequestPhoneNumberChangeOtpCommandResult>;

public sealed record RequestPhoneNumberChangeOtpCommandResult(string DevelopmentCode);