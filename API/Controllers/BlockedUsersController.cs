using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Users.Blocking.Commands.BlockUser;
using Messenger.Application.Features.Users.Blocking.Commands.UnblockUser;
using Messenger.Application.Features.Users.Blocking.Queries.GetBlockedUsers;
using Messenger.Application.Features.Users.Dtos;

namespace Messenger.API.Controllers;

[Authorize]
[ApiController]
[Route("api/blocked-users")]
public sealed class BlockedUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlockedUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{userId:guid}")]
    public async Task<ActionResult<Result>> BlockUser(Guid userId)
    {
        var result = await _mediator.Send(new BlockUserCommand(userId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult<Result>> UnblockUser(Guid userId)
    {
        var result = await _mediator.Send(new UnblockUserCommand(userId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<BlockedUserDto>>>> GetBlockedUsers()
    {
        var result = await _mediator.Send(new GetBlockedUsersQuery());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}