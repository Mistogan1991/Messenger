using Messenger.Application.Common.Messaging;
using Messenger.Application.Features.Messages.EventHandlers;
using Messenger.Contracts.IntegrationEvents;
using Messenger.Domain.Events.Messages;
using Xunit;

namespace Messenger.UnitTests.Application;

public class PublishMessageSentIntegrationEventHandlerTests
{
    private sealed class CapturingPublisher : IIntegrationEventPublisher
    {
        public object? Published { get; private set; }
        public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken ct = default) where TEvent : class
        {
            Published = integrationEvent;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Publishes_a_MessageSentIntegrationEvent_mapped_from_the_domain_event()
    {
        var domainEvent = new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var publisher = new CapturingPublisher();
        var handler = new PublishMessageSentIntegrationEventHandler(publisher);

        await handler.Handle(new DomainEventNotification<MessageSentEvent>(domainEvent), CancellationToken.None);

        var published = Assert.IsType<MessageSentIntegrationEvent>(publisher.Published);
        Assert.Equal(domainEvent.MessageId, published.MessageId);
        Assert.Equal(domainEvent.ChatId, published.ChatId);
        Assert.Equal(domainEvent.SenderId, published.SenderId);
        Assert.Equal(domainEvent.OccurredOnUtc, published.OccurredOnUtc);
    }
}
