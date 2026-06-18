using Messenger.Application.Common.Messaging;
using Messenger.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messenger.Persistence.Outbox;

/// <summary>
/// Background service that polls the outbox and publishes unprocessed domain events at-least-once.
/// A message that keeps failing is parked once <see cref="OutboxOptions.MaxRetries"/> is reached.
/// </summary>
public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        IOptions<OutboxOptions> options,
        ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

                await ProcessBatchAsync(dbContext, dispatcher, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processing loop failed.");
            }

            try
            {
                await Task.Delay(_options.PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Publishes one batch of pending messages. Exposed (non-private) so it can be exercised in tests
    /// without the hosting loop. Returns the number of messages attempted.
    /// </summary>
    public async Task<int> ProcessBatchAsync(
        ApplicationDbContext dbContext,
        IDomainEventDispatcher dispatcher,
        CancellationToken ct)
    {
        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null && m.RetryCount < _options.MaxRetries)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        if (messages.Count == 0)
            return 0;

        foreach (var message in messages)
        {
            try
            {
                var domainEvent = message.Deserialize();
                await dispatcher.DispatchAsync([domainEvent], ct);
                message.MarkProcessed(DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.ToString());
                _logger.LogError(ex, "Failed to publish outbox message {OutboxMessageId} (attempt {RetryCount}).",
                    message.Id, message.RetryCount);
            }
        }

        await dbContext.SaveChangesAsync(ct);
        return messages.Count;
    }
}
