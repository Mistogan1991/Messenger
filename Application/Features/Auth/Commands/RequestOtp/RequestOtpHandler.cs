using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Security;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Auth;
using Messenger.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Messenger.Application.Features.Auth.Commands.RequestOtp;

public sealed class RequestOtpHandler : IAppRequestHandler<RequestOtpCommand, RequestOtpCommandResult>
{
    private readonly JwtSettings _jwtSetting;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOtpProvider _otpProvider;
    private readonly IHashProvider _hashProvider;
    private readonly IOtpCodeRepository _otpRepository;

    public RequestOtpHandler(
        IUnitOfWork unitOfWork,
        IOtpProvider otpProvider,
        IHashProvider hashProvider,
        IOptions<AppSettings> options,
        IOtpCodeRepository otpRepository)
    {
        _unitOfWork = unitOfWork;
        _otpProvider = otpProvider;
        _hashProvider = hashProvider;
        _otpRepository = otpRepository;
        _jwtSetting = options.Value!.JwtSettings;
    }

    public async Task<Result<RequestOtpCommandResult>> Handle(RequestOtpCommand request, CancellationToken ct)
    {
        var lastOtp = await _otpRepository.GetLastIssuedAsync(request.PhoneNumber, ct);

        if (lastOtp is not null)
        {
            var seconds = (DateTime.UtcNow - lastOtp.CreatedAtUtc).TotalSeconds;

            if (seconds < _jwtSetting.CooldownSeconds)
                return Result<RequestOtpCommandResult>.Failure([
                    $"Try again after {_jwtSetting.CooldownSeconds} seconds."]);
        }

        var code = _otpProvider.Generate(_jwtSetting.CodeLength);

        var hash = _hashProvider.Hash(code);

        var otp = OtpCode.Create(
            request.PhoneNumber,
            hash,
            OtpPurpose.Login,
            DateTime.UtcNow.AddMinutes(2));

        await _otpRepository.AddAsync(otp, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        await _otpProvider.SendAsync(request.PhoneNumber, code, ct);

        return Result<RequestOtpCommandResult>.Success(new RequestOtpCommandResult(code));
    }
}