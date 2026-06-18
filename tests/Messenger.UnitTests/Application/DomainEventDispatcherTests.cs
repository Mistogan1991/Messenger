using MediatR;
using Messenger.Application.Common.Messaging;
using Messenger.Domain.Common;
using Messenger.Domain.Events.Messages;
using Xunit;

namespace Messenger.UnitTests.Application;

public class DomainEventDispatcherTests
{
    private sealed class CapturingPublisher : IPublisher
    {
        public List<object> Published { get; } = [];

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            Published.Add(notification);
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            Published.Add(notification!);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Wraps_each_domain_event_in_a_closed_notification()
    {
        var publisher = new CapturingPublisher();
        var dispatcher = new DomainEventDispatcher(publisher);
        var evt = new MessageSentEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        await dispatcher.DispatchAsync([evt]);

        var published = Assert.Single(publisher.Published);
        var wrapper = Assert.IsType<DomainEventNotification<MessageSentEvent>>(published);
        Assert.Same(evt, wrapper.DomainEvent);
    }

    [Fact]
    public async Task Publishes_nothing_for_empty_event_set()
    {
        var publisher = new CapturingPublisher();
        var dispatcher = new DomainEventDispatcher(publisher);

        await dispatcher.DispatchAsync([]);

        Assert.Empty(publisher.Published);
    }
}
