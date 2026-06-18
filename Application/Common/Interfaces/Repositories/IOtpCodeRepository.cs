using Messenger.Domain.Enums;

namespace Messenger.Domain.Aggregates.Auth;

public interface IOtpCodeRepository
{
    Task AddAsync(OtpCode otpCode, CancellationToken ct);

    Task<OtpCode?> GetLatestAsync(string phoneNumber, OtpPurpose purpose, CancellationToken ct);

    Task<OtpCode?> GetLastIssuedAsync(string phoneNumber, CancellationToken ct);
}
