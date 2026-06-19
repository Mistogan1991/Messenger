using System.Security.Claims;
using Messenger.Application.Common.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.API.Hubs;

/// <summary>
/// Realtime chat hub. Clients call <c>JoinChat</c> for each chat they want live updates from and
/// receive a <c>MessageSent</c> event when a new message arrives. Membership is enforced on join.
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
        var userId = GetUserId();

        if (!await _chatRepository.IsParticipantAsync(chatId, userId, Context.ConnectionAborted))
            throw new HubException("You are not a member of this chat.");

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(chatId), Context.ConnectionAborted);
    }

    public Task LeaveChat(Guid chatId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(chatId), Context.ConnectionAborted);

    private Guid GetUserId()
    {
        var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new HubException("Unauthorized.");
    }
}
