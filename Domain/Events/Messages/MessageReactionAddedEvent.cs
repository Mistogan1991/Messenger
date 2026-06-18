using Messenger.Domain.Common;

namespace Messenger.Domain.Events.Messages;

public sealed record MessageReactionAddedEvent(Guid MessageId, Guid UserId, string Emoji) : DomainEvent;
