using Messenger.Domain.Common;

namespace Messenger.Domain.Events.Messages;

public sealed record MessageSentEvent(Guid MessageId, Guid ChatId, Guid SenderId) : DomainEvent;
