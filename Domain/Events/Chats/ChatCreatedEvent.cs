using Messenger.Domain.Common;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Events.Chats;

public sealed record ChatCreatedEvent(Guid ChatId, ChatType Type, Guid CreatedByUserId) : DomainEvent;
