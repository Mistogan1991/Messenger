using MediatR;
using Messenger.API.Contracts.Profile;
using Messenger.API.Mappings;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Users.Dtos;
using Messenger.Application.Features.Users.Privacy.Queries.GetPrivacySettings;
using Messenger.Application.Features.Users.Profile.Queries.GetMyProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPut("request-phone-number-change-otp")]
    public async Task<ActionResult<Result>> RequestPhoneNumberChangeOtp(RequestPhoneNumberChangeOtpRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("confirm-phone-number-change")]
    public async Task<ActionResult<Result>> ConfirmPhoneNumberChange(ConfirmPhoneNumberChangeRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("username")]
    public async Task<ActionResult<Result>> SetUserName(SetUsernameRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet()]
    public async Task<ActionResult<Result<MyProfileDto>>> GetMyProfile()
    {
        var result = await _mediator.Send(new GetMyProfileQuery());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut()]
    public async Task<ActionResult<Result>> UpdateMyProfile(UpdateMyProfileRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("privacy")]
    public async Task<ActionResult<Result<PrivacySettingsDto>>> GetPrivacy()
    {
        var result = await _mediator.Send(new GetPrivacySettingsQuery());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("privacy")]
    public async Task<ActionResult<Result>> UpdatePrivacy(UpdatePrivacySettingsRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
