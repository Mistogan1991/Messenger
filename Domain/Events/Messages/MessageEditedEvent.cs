using Messenger.Domain.Common;

namespace Messenger.Domain.Events.Messages;

public sealed record MessageEditedEvent(Guid MessageId, Guid ChatId) : DomainEvent;
