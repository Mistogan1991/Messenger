using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Auth.Commands.RevokeSession;

public sealed record RevokeSessionCommand() : IAppRequest;
