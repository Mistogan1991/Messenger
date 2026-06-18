using MediatR;
using Messenger.API.Contracts.Messages;
using Messenger.API.Mappings;
using Messenger.Application.Features.Messages.Commands.DeleteMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

[Authorize]
[ApiController]
[Route("api/messages")]
public sealed class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Send(SendMessageRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{messageId}")]
    public async Task<IActionResult> Edit(Guid messageId, EditMessageRequest request)
    {
        var result = await _mediator.Send(request.ToCommand(messageId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> Delete(Guid messageId)
    {
        var result = await _mediator.Send(new DeleteMessageCommand(messageId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("reply")]
    public async Task<IActionResult> Reply(ReplyMessageRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("forward")]
    public async Task<IActionResult> Forward(ForwardMessageRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("read")]
    public async Task<IActionResult> Read(ReadMessageRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("reaction")]
    public async Task<IActionResult> AddReaction(AddReactionRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("reaction")]
    public async Task<IActionResult> RemoveReaction(RemoveReactionRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("attachment")]
    public async Task<IActionResult> AddAttachment(AddAttachmentRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
