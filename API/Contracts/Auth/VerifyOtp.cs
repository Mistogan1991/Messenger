namespace Messenger.API.Contracts.Auth;

public sealed record VerifyOtpRequest(
    string PhoneNumber,
    string Code,
    string DeviceId,
    string DeviceName,
    string DeviceType
);