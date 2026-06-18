
using MediatR;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Users.Profile.Queries.GetUserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("user-profile/{id}")]
    public async Task<ActionResult<Result<UserProfileDto>>> GetUserProfile(Guid id)
    {
        var result = await _mediator.Send(new GetUserProfileQuery(id));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
