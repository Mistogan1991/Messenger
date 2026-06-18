using MediatR;
using Messenger.Domain.Common;

namespace Messenger.Application.Common.Messaging;

/// <summary>
/// MediatR envelope around a domain event. This keeps the Domain layer free of any MediatR
/// dependency: aggregates raise pure <see cref="IDomainEvent"/>s and the dispatcher wraps each
/// one in a closed <c>DomainEventNotification&lt;TDomainEvent&gt;</c> before publishing.
/// Handlers subscribe via <c>INotificationHandler&lt;DomainEventNotification&lt;TDomainEvent&gt;&gt;</c>.
/// </summary>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
