using Messenger.Domain.Common;

namespace Messenger.Domain.Events.Messages;

public sealed record MessageDeletedEvent(Guid MessageId, Guid ChatId) : DomainEvent;
