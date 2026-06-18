using Messenger.Domain.Common;

namespace Messenger.Domain.Events.Chats;

public sealed record ParticipantAddedEvent(Guid ChatId, Guid UserId) : DomainEvent;
