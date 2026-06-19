using Messenger.API.Hubs;
using Messenger.Application.Abstractions.Realtime;
using Messenger.Application.Features.Messages.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.API.Realtime;

public sealed class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRRealtimeNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task MessageSentAsync(Guid chatId, MessageDto message, CancellationToken ct = default)
        => _hubContext.Clients
            .Group(ChatHub.GroupName(chatId))
            .SendAsync("MessageSent", message, ct);
}
