namespace Messenger.Contracts.IntegrationEvents;

/// <summary>
/// Published to the message bus when a message is sent. A stable cross-service contract that
/// downstream consumers (notifications, search indexing, analytics) subscribe to.
/// </summary>
public sealed record MessageSentIntegrationEvent(
    Guid MessageId,
    Guid ChatId,
    Guid SenderId,
    DateTime OccurredOnUtc);
