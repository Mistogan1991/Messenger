namespace Messenger.Persistence.Outbox;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    /// <summary>How often the processor polls for unpublished messages.</summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>Maximum messages published per poll.</summary>
    public int BatchSize { get; set; } = 20;

    /// <summary>Attempts before a failing message is parked (left unprocessed, no longer retried).</summary>
    public int MaxRetries { get; set; } = 3;
}
