using Messenger.Application.Features.Messages.Dtos;

namespace Messenger.Application.Abstractions.Realtime;

/// <summary>
/// Pushes realtime events to connected clients. Implemented over SignalR in the API layer, keeping
/// the Application layer free of any transport dependency.
/// </summary>
public interface IRealtimeNotifier
{
    /// <summary>Notifies the members of a chat that a new message was sent.</summary>
    Task MessageSentAsync(Guid chatId, MessageDto message, CancellationToken ct = default);
}
