using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Security;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Auth;
using Messenger.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Messenger.Application.Features.Users.Profile.Commands.RequestPhoneNumberChangeOtp;

public class RequestPhoneNumberChangeOtpHandler
    : IAppRequestHandler<RequestPhoneNumberChangeOtpCommand, RequestPhoneNumberChangeOtpCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;
    private readonly IOtpProvider _otpProvider;
    private readonly IHashProvider _hashProvider;
    private readonly IUserRepository _userRepository;
    private readonly IOtpCodeRepository _otpRepository;

    public RequestPhoneNumberChangeOtpHandler(
        IUnitOfWork unitOfWork,
        IOtpProvider otpProvider,
        IHashProvider hashProvider,
        IOptions<AppSettings> options,
        IUserRepository userRepository,
        IOtpCodeRepository otpRepository)
    {
        _unitOfWork = unitOfWork;
        _otpProvider = otpProvider;
        _hashProvider = hashProvider;
        _otpRepository = otpRepository;
        _userRepository = userRepository;
        _jwtSettings = options.Value!.JwtSettings;
    }

    public async Task<Result<RequestPhoneNumberChangeOtpCommandResult>> Handle(RequestPhoneNumberChangeOtpCommand request, CancellationToken ct)
    {
        var existingUser = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, ct);

        if (existingUser is not null)
        {
            return Result<RequestPhoneNumberChangeOtpCommandResult>.Failure(["Phone number already used."]);
        }

        var lastOtp = await _otpRepository.GetLastIssuedAsync(request.PhoneNumber, ct);

        if (lastOtp is not null)
        {
            var seconds = (DateTime.UtcNow - lastOtp.CreatedAtUtc).TotalSeconds;

            if (seconds < _jwtSettings.CooldownSeconds)
                return Result<RequestPhoneNumberChangeOtpCommandResult>.Failure([
                    $"Try again after {_jwtSettings.CooldownSeconds} seconds."]);
        }

        var code = _otpProvider.Generate(_jwtSettings.CodeLength);

        var hash = _hashProvider.Hash(code);

        var otp = OtpCode.Create(
            request.PhoneNumber,
            hash,
            OtpPurpose.ChangePhone,
            DateTime.UtcNow.AddMinutes(2));

        await _otpRepository.AddAsync(otp, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        await _otpProvider.SendAsync(request.PhoneNumber, code, ct);

        return Result<RequestPhoneNumberChangeOtpCommandResult>.Success(
            new RequestPhoneNumberChangeOtpCommandResult(code));
    }
}
