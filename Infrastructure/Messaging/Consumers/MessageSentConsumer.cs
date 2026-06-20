using MassTransit;
using Messenger.Contracts.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace Messenger.Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumes <see cref="MessageSentIntegrationEvent"/> from the bus. Currently a placeholder seam
/// for downstream fan-out (push notifications, search indexing); for now it just records receipt.
/// </summary>
public sealed class MessageSentConsumer : IConsumer<MessageSentIntegrationEvent>
{
    private readonly ILogger<MessageSentConsumer> _logger;

    public MessageSentConsumer(ILogger<MessageSentConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<MessageSentIntegrationEvent> context)
    {
        _logger.LogInformation(
            "Integration event consumed: message {MessageId} sent in chat {ChatId} by {SenderId}.",
            context.Message.MessageId, context.Message.ChatId, context.Message.SenderId);

        return Task.CompletedTask;
    }
}
