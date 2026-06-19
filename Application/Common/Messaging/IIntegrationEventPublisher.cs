namespace Messenger.Application.Common.Messaging;

/// <summary>
/// Publishes integration events to the message bus. Implemented over MassTransit in Infrastructure,
/// keeping the Application layer free of any transport dependency.
/// </summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken ct = default)
        where TEvent : class;
}
