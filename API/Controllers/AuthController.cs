using MediatR;
using Messenger.API.Contracts.Auth;
using Messenger.API.Mappings;
using Messenger.Application.Abstractions.Authentication;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Auth.Commands.Logout;
using Messenger.Application.Features.Auth.Commands.RefreshToken;
using Messenger.Application.Features.Auth.Commands.RequestOtp;
using Messenger.Application.Features.Auth.Commands.RevokeSession;
using Messenger.Application.Features.Auth.Commands.VerifyOtp;
using Messenger.Application.Features.Auth.Queries.GetSessions;
using Messenger.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Messenger.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EnableRateLimiting(RateLimitingExtensions.OtpPolicy)]
    [HttpPost("request-otp")]
    public async Task<ActionResult<Result<RequestOtpCommandResult>>> RequestOtp(RequestOtpRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("verify-otp")]
    public async Task<ActionResult<Result<VerifyOtpCommandResult>>> VerifyOtp(VerifyOtpRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<Result<RefreshTokenCommandResult>>> RefreshToken(
        RefreshTokenRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<Result>> Logout()
    {
        var result = await _mediator.Send(new LogoutCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("revoke-session")]
    public async Task<ActionResult<Result>> RevokeSession()
    {
        var result = await _mediator.Send(new RevokeSessionCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<ActionResult<Result<List<SessionDto>>>> GetSessions()
    {
        var result = await _mediator.Send(new GetSessionsQuery());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
