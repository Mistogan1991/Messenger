using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Profile.Commands.SetUsername;

public sealed record SetUsernameCommand(string Username) : IAppRequest;
