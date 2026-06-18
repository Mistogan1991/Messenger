namespace Messenger.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
