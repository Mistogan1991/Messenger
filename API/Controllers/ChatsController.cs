using MediatR;
using Messenger.API.Contracts.Chats;
using Messenger.API.Mappings;
using Messenger.Application.Features.Chats.Commands.ArchiveChat;
using Messenger.Application.Features.Chats.Commands.JoinChannel;
using Messenger.Application.Features.Chats.Commands.LeaveChat;
using Messenger.Application.Features.Chats.Commands.MuteChat;
using Messenger.Application.Features.Chats.Commands.PinChat;
using Messenger.Application.Features.Chats.Commands.PromoteAdmin;
using Messenger.Application.Features.Chats.Commands.RemoveMember;
using Messenger.Application.Features.Chats.Commands.StartPrivateChat;
using Messenger.Application.Features.Chats.Commands.UnArchiveChat;
using Messenger.Application.Features.Chats.Commands.UnMuteChat;
using Messenger.Application.Features.Chats.Commands.UnPinChat;
using Messenger.Application.Features.Chats.Queries.GetChatInfo;
using Messenger.Application.Features.Chats.Queries.GetChatMembers;
using Messenger.Application.Features.Chats.Queries.GetMyChats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

[ApiController]
[Authorize]
[Route("api/chats")]
public class ChatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("groups")]
    public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("channels")]
    public async Task<IActionResult> CreateChannel(CreateChannelRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("private/{userId:guid}")]
    public async Task<IActionResult> StartPrivate(Guid userId)
    {
        var result = await _mediator.Send(new StartPrivateChatCommand(userId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{chatId}/join")]
    public async Task<IActionResult> Join(Guid chatId)
    {
        var result = await _mediator.Send(new JoinChannelCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{chatId}/leave")]
    public async Task<IActionResult> Leave(Guid chatId)
    {
        var result = await _mediator.Send(new LeaveChatCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{chatId}/members")]
    public async Task<IActionResult> AddMembers(Guid chatId, AddMembersRequest request)
    {
        var result = await _mediator.Send(request.ToCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{chatId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(Guid chatId, Guid userId)
    {
        var result = await _mediator.Send(new RemoveMemberCommand(chatId, userId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{chatId}/promote-admin/{userId}")]
    public async Task<IActionResult> PromoteAdmin(Guid chatId, Guid userId)
    {
        var result = await _mediator.Send(new PromoteAdminCommand(chatId, userId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{chatId}/demote-admin/{userId}")]
    public async Task<IActionResult> DemoteAdmin(Guid chatId, Guid userId)
    {
        var result = await _mediator.Send(new DemoteAdminCommand(chatId, userId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyChats()
    {
        var result = await _mediator.Send(new GetMyChatsQuery());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetInfo(Guid chatId)
    {
        var result = await _mediator.Send(new GetChatInfoQuery(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{chatId:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid chatId)
    {
        var result = await _mediator.Send(new GetChatMembersQuery(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{chatId}")]
    public async Task<IActionResult> EditInfo(Guid chatId, EditChatRequest request)
    {
        var result = await _mediator.Send(request.ToCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{chatId:guid}/mute")]
    public async Task<IActionResult> Mute(Guid chatId)
    {
        var result = await _mediator.Send(new MuteChatCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{chatId:guid}/unmute")]
    public async Task<IActionResult> UnMute(Guid chatId)
    {
        var result = await _mediator.Send(new UnMuteChatCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{chatId:guid}/archive")]
    public async Task<IActionResult> Archive(Guid chatId)
    {
        var result = await _mediator.Send(new ArchiveChatCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{chatId:guid}/unarchive")]
    public async Task<IActionResult> UnArchive(Guid chatId)
    {
        var result = await _mediator.Send(new UnArchiveChatCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{chatId:guid}/pin")]
    public async Task<IActionResult> Pin(Guid chatId)
    {
        var result = await _mediator.Send(new PinChatCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{chatId:guid}/unpin")]
    public async Task<IActionResult> UnPin(Guid chatId)
    {
        var result = await _mediator.Send(new UnPinChatCommand(chatId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
