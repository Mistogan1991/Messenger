using System.Security.Claims;
using Messenger.Application.Common.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.API.Hubs;

/// <summary>
/// Realtime chat hub. Clients call <c>JoinChat</c> for each chat they want live updates from and
/// receive a <c>MessageSent</c> event when a new message arrives, plus <c>UserTyping</c> /
/// <c>UserStoppedTyping</c> from other members. Membership is enforced on every operation.
/// </summary>
[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;

    public ChatHub(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public static string GroupName(Guid chatId) => $"chat:{chatId}";

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
