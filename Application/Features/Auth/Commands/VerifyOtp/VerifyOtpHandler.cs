using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Security;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Auth;
using Messenger.Domain.Aggregates.Chats;
using Messenger.Domain.Aggregates.Users;
using Messenger.Domain.Enums;
using Messenger.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace Messenger.Application.Features.Auth.Commands.VerifyOtp;

public sealed class VerifyOtpHandler : IAppRequestHandler<VerifyOtpCommand, VerifyOtpCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;
    private readonly IJwtProvider _jwtProvider;
    private readonly IHashProvider _hashProvider;
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOtpCodeRepository _otpRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public VerifyOtpHandler(
        IUnitOfWork unitOfWork,
        IJwtProvider jwtProvider,
        IHashProvider hashProvider,
        IOptions<AppSettings> options,
        IChatRepository chatRepository,
        IUserRepository userRepository,
        IOtpCodeRepository otpRepository,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
        _hashProvider = hashProvider;
        _otpRepository = otpRepository;
        _chatRepository = chatRepository;
        _userRepository = userRepository;
        _jwtSettings = options.Value!.JwtSettings;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<Result<VerifyOtpCommandResult>> Handle(VerifyOtpCommand request, CancellationToken ct)
    {
        var otp = await _otpRepository.GetLatestAsync(request.PhoneNumber, OtpPurpose.Login, ct);

        if (otp is null)
            return Result<VerifyOtpCommandResult>.Failure(["OTP not found."]);

        if (otp.IsExpired())
            return Result<VerifyOtpCommandResult>.Failure(["OTP expired."]);

        if (otp.Attempts >= _jwtSettings.MaxAttempts)
            return Result<VerifyOtpCommandResult>.Failure(["OTP locked."]);

        var hash = _hashProvider.Hash(request.Code);

        if (otp.CodeHash != hash)
        {
            otp.IncreaseAttempt();

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<VerifyOtpCommandResult>.Failure(["Invalid code."]);
        }

        otp.MarkAsUsed();

        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, ct);

        var isNewUser = false;

        if (user is null)
        {
            isNewUser = true;

            user = User.Create(PhoneNumber.Create(request.PhoneNumber));

            await _userRepository.AddAsync(user, ct);

            var savedMessages = Chat.CreateSavedMessages(user.Id);

            await _chatRepository.AddAsync(savedMessages, ct);
        }

        var refreshToken = _refreshTokenGenerator.Generate();

        var refreshTokenHash = _hashProvider.Hash(refreshToken);

        var session = user.AddSession(
            request.DeviceId,
            request.DeviceName,
            request.DeviceType,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        );

        var accessToken = _jwtProvider.Generate(user, session.Id);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<VerifyOtpCommandResult>.Success(
            new VerifyOtpCommandResult(
                user.Id,
                accessToken,
                refreshToken,
                isNewUser)
            );
    }
}
