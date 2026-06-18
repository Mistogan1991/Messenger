using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Abstractions.Security;
using Messenger.Application.Common.CQRS;
using Messenger.Application.Common.Interfaces.Persistence;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace Messenger.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenHandler : IAppRequestHandler<RefreshTokenCommand, RefreshTokenCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;
    private readonly IJwtProvider _jwtProvider;
    private readonly ICurrentUser _currentUser;
    private readonly IHashProvider _hashProvider;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public RefreshTokenHandler(
        IUnitOfWork unitOfWork,
        IJwtProvider jwtProvider,
        ICurrentUser currentUser,
        IHashProvider hashProvider,
        IOptions<AppSettings> options,
        IUserRepository userRepository,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
        _currentUser = currentUser;
        _hashProvider = hashProvider;
        _userRepository = userRepository;
        _jwtSettings = options.Value!.JwtSettings;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<Result<RefreshTokenCommandResult>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetBySessionAsync(_currentUser.SessionId, ct);

        if (user is null)
            return Result<RefreshTokenCommandResult>.Failure(["user with session not found."]);

        var session = user.GetSession(_currentUser.SessionId);

        if (session is null)
            return Result<RefreshTokenCommandResult>.Failure(["Session not found."]);

        var refreshTokenHash = _hashProvider.Hash(request.RefreshToken);

        if (session.RefreshTokenHash != refreshTokenHash)
            return Result<RefreshTokenCommandResult>.Failure(["Refresh token is not valid."]);

        if (!session.IsActive())
            return Result<RefreshTokenCommandResult>.Failure(["Session expired or revoked."]);

        var newRefreshToken = _refreshTokenGenerator.Generate();
        var newRefreshHash = _hashProvider.Hash(newRefreshToken);

        var newExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays);

        session.ReplaceRefreshToken(newRefreshHash, newExpiry);
        session.UpdateActivity();

        var newAccessToken = _jwtProvider.Generate(user, session.Id);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<RefreshTokenCommandResult>.Success(
            new RefreshTokenCommandResult(newAccessToken, newRefreshToken));
    }
}
