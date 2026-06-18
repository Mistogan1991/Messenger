using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand() : IAppRequest;
