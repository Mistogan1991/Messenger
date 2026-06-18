using Messenger.API.Contracts.Auth;
using Messenger.Application.Features.Auth.Commands.RefreshToken;
using Messenger.Application.Features.Auth.Commands.RequestOtp;
using Messenger.Application.Features.Auth.Commands.VerifyOtp;

namespace Messenger.API.Mappings;

public static class AuthMappings
{
    public static RequestOtpCommand ToCommand(this RequestOtpRequest request)
    {
        return new RequestOtpCommand(request.PhoneNumber);
    }

    public static VerifyOtpCommand ToCommand(this VerifyOtpRequest request)
    {
        return new VerifyOtpCommand(
            request.PhoneNumber,
            request.Code,
            request.DeviceId,
            request.DeviceName,
            request.DeviceType);
    }

    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request)
    {
        return new RefreshTokenCommand(request.RefreshToken);
    }
}
