using Messenger.Domain.Common;

namespace Messenger.Domain.Events.Chats;

public sealed record ParticipantRemovedEvent(Guid ChatId, Guid UserId) : DomainEvent;
