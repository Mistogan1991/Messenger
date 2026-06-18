using Messenger.Domain.Aggregates.Auth;
using Messenger.Domain.Enums;
using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public sealed class OtpCodeRepository : IOtpCodeRepository
{
    private readonly ApplicationDbContext _context;

    public OtpCodeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OtpCode otpCode, CancellationToken ct)
    {
        await _context.OtpCodes.AddAsync(otpCode, ct);
    }

    public async Task<OtpCode?> GetLatestAsync(string phoneNumber, OtpPurpose purpose, CancellationToken ct)
    {
        return await _context.OtpCodes
            .Where(x =>
                x.PhoneNumber == phoneNumber &&
                x.Purpose == purpose)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<OtpCode?> GetLastIssuedAsync(string phoneNumber, CancellationToken ct)
    {
        return await _context.OtpCodes
            .Where(x => x.PhoneNumber == phoneNumber)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(ct);
    }
}
