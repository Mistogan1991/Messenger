using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Auth.Commands.VerifyOtp
{
    public sealed record VerifyOtpCommand(
        string PhoneNumber,
        string Code,
        string DeviceId,
        string DeviceName,
        string DeviceType
    ) : IAppRequest<VerifyOtpCommandResult>;

    public sealed record VerifyOtpCommandResult(
        Guid UserId,
        string AccessToken,
        string RefreshToken,
        bool IsNewUser
    );
}
