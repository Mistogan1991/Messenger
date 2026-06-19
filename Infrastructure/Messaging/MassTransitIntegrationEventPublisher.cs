using MassTransit;
using Messenger.Application.Common.Messaging;

namespace Messenger.Infrastructure.Messaging;

public sealed class MassTransitIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitIntegrationEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken ct = default)
        where TEvent : class
        => _publishEndpoint.Publish(integrationEvent, ct);
}
