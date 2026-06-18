using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Security;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Messenger.Domain.Aggregates.Auth;
using Messenger.Domain.Enums;
using Messenger.Domain.ValueObjects;
using Microsoft.Extensions.Options;

namespace Messenger.Application.Features.Users.Profile.Commands.ConfirmPhoneNumberChange;

public sealed class ConfirmPhoneNumberChangeHandler : IAppRequestHandler<ConfirmPhoneNumberChangeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;
    private readonly ICurrentUser _currentUser;
    private readonly IHashProvider _hashProvider;
    private readonly IUserRepository _userRepository;
    private readonly IOtpCodeRepository _otpRepository;

    public ConfirmPhoneNumberChangeHandler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IHashProvider hashProvider,
        IOptions<AppSettings> options,
        IUserRepository userRepository,
        IOtpCodeRepository otpRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _hashProvider = hashProvider;
        _otpRepository = otpRepository;
        _userRepository = userRepository;
        _jwtSettings = options.Value!.JwtSettings;
    }

    public async Task<Result> Handle(ConfirmPhoneNumberChangeCommand request, CancellationToken ct)
    {
        var otp = await _otpRepository.GetLatestAsync(request.PhoneNumber, OtpPurpose.ChangePhone, ct);

        if (otp is null)
            return Result.Failure(["OTP not found."]);

        if (otp.IsExpired())
            return Result.Failure(["OTP expired."]);

        if (otp.Attempts >= _jwtSettings.MaxAttempts)
            return Result.Failure(["OTP locked."]);

        var hash = _hashProvider.Hash(request.Code);

        if (otp.CodeHash != hash)
        {
            otp.IncreaseAttempt();

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Failure(["Invalid code."]);
        }

        otp.MarkAsUsed();

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, ct);

        if (user is null)
            return Result.Failure(["User not found."]);

        var exists = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, ct);

        if (exists is not null && exists.Id != user.Id)
        {
            return Result.Failure(["Phone number already registered."]);
        }

        user.ChangePhoneNumber(PhoneNumber.Create(request.PhoneNumber));

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
