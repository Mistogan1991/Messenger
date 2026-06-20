using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Users.Presence.Commands.MarkUserOffline;

/// <summary>Records that a user went offline by stamping their last-seen time.</summary>
public sealed record MarkUserOfflineCommand(Guid UserId) : IAppRequest;
