using System.Security.Claims;
using MediatR;
using Messenger.Application.Abstractions.Realtime;
using Messenger.Application.Common.Interfaces.Repositories;
using Messenger.Application.Features.Users.Presence.Commands.MarkUserOffline;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.API.Hubs;

/// <summary>
/// Realtime chat hub. Clients call <c>JoinChat</c> for each chat they want live updates from and
/// receive a <c>MessageSent</c> event when a new message arrives, plus <c>UserTyping</c> /
/// <c>UserStoppedTyping</c> from other members. Membership is enforced on every operation.
/// Online presence is tracked across the connection lifecycle.
/// </summary>
[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;
    private readonly IPresenceTracker _presenceTracker;
    private readonly ISender _mediator;

    public ChatHub(IChatRepository chatRepository, IPresenceTracker presenceTracker, ISender mediator)
    {
        _chatRepository = chatRepository;
        _presenceTracker = presenceTracker;
        _mediator = mediator;
    }

    public static string GroupName(Guid chatId) => $"chat:{chatId}";

    public override async Task OnConnectedAsync()
    {
        await _presenceTracker.UserConnectedAsync(GetUserId());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();

        if (await _presenceTracker.UserDisconnectedAsync(userId))
            await _mediator.Send(new MarkUserOfflineCommand(userId));

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChat(Guid chatId)
    {
        await EnsureParticipantAsync(chatId);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(chatId), Context.ConnectionAborted);
    }

    public Task LeaveChat(Guid chatId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(chatId), Context.ConnectionAborted);

    /// <summary>Tells the other members of the chat that this user started typing.</summary>
    public async Task Typing(Guid chatId)
    {
        var userId = await EnsureParticipantAsync(chatId);
        await Clients.OthersInGroup(GroupName(chatId)).SendAsync("UserTyping", chatId, userId, Context.ConnectionAborted);
    }

    /// <summary>Tells the other members of the chat that this user stopped typing.</summary>
    public async Task StopTyping(Guid chatId)
    {
        var userId = await EnsureParticipantAsync(chatId);
        await Clients.OthersInGroup(GroupName(chatId)).SendAsync("UserStoppedTyping", chatId, userId, Context.ConnectionAborted);
    }

    private async Task<Guid> EnsureParticipantAsync(Guid chatId)
    {
        var userId = GetUserId();

        if (!await _chatRepository.IsParticipantAsync(chatId, userId, Context.ConnectionAborted))
            throw new HubException("You are not a member of this chat.");

        return userId;
    }

    private Guid GetUserId()
    {
        var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new HubException("Unauthorized.");
    }
}
