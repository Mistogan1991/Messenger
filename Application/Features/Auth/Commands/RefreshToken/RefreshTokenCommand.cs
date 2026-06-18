using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : IAppRequest<RefreshTokenCommandResult>;

public sealed record RefreshTokenCommandResult(string AccessToken, string RefreshToken);
