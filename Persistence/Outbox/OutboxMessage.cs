using System.Text.Json;
using Messenger.Domain.Common;

namespace Messenger.Persistence.Outbox;

/// <summary>
/// A durably-stored domain event awaiting publication. Written in the same transaction as the
/// aggregate changes that raised it (transactional outbox), then published at-least-once by the
/// <see cref="OutboxProcessor"/>. This is a persistence concern, not a domain aggregate.
/// </summary>
public sealed class OutboxMessage
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.General);

    public Guid Id { get; private set; }

    /// <summary>The domain event's CLR <see cref="System.Type.FullName"/> (assembly-version independent).</summary>
    public string Type { get; private set; } = default!;

    /// <summary>The JSON-serialized domain event payload.</summary>
    public string Content { get; private set; } = default!;

    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public int RetryCount { get; private set; }

    public string? Error { get; private set; }

    private OutboxMessage() { }

    private OutboxMessage(Guid id, string type, string content, DateTime occurredOnUtc)
    {
        Id = id;
        Type = type;
        Content = content;
        OccurredOnUtc = occurredOnUtc;
    }

    public static OutboxMessage Create(IDomainEvent domainEvent)
    {
        var clrType = domainEvent.GetType();

        return new OutboxMessage(
            Guid.NewGuid(),
            clrType.FullName!,
            JsonSerializer.Serialize(domainEvent, clrType, SerializerOptions),
            domainEvent.OccurredOnUtc);
    }

    /// <summary>Reconstructs the original domain event. Throws if its type can no longer be resolved.</summary>
    public IDomainEvent Deserialize()
    {
        var clrType = DomainEventTypeResolver.Resolve(Type)
            ?? throw new InvalidOperationException($"Cannot resolve outbox event type '{Type}'.");

        return (IDomainEvent)JsonSerializer.Deserialize(Content, clrType, SerializerOptions)!;
    }

    public void MarkProcessed(DateTime utcNow)
    {
        ProcessedOnUtc = utcNow;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        RetryCount++;
        Error = error;
    }
}
