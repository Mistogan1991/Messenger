using System.Collections.Concurrent;
using Messenger.Domain.Common;

namespace Messenger.Persistence.Outbox;

/// <summary>
/// Resolves a domain event's stored <see cref="System.Type.FullName"/> back to its CLR type by
/// scanning the Domain assembly. Avoids storing assembly-qualified names (which embed a version)
/// so outbox rows survive assembly version bumps.
/// </summary>
internal static class DomainEventTypeResolver
{
    private static readonly ConcurrentDictionary<string, Type?> Cache = new();

    public static Type? Resolve(string fullName) =>
        Cache.GetOrAdd(fullName, name => typeof(IDomainEvent).Assembly.GetType(name));
}
