using MediatR;
using Messenger.API.Contracts.Contacts;
using Messenger.API.Mappings;
using Messenger.Application.Common.Models;
using Messenger.Application.Features.Contacts.Commands.DeleteContact;
using Messenger.Application.Features.Contacts.Dtos;
using Messenger.Application.Features.Contacts.Queries.GetContacts;
using Messenger.Application.Features.Contacts.Queries.SearchContacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

[Authorize]
[ApiController]
[Route("api/contacts")]
public sealed class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Result>> AddContact(AddContactRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<Result>> UpdateContact(UpdateContactRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{contactUserId:guid}")]
    public async Task<ActionResult<Result>> DeleteContact(Guid contactUserId)
    {
        var result = await _mediator.Send(new DeleteContactCommand(contactUserId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<ContactDto>>>> GetContacts()
    {
        var result = await _mediator.Send(new GetContactsQuery());

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<Result<List<ContactDto>>>> SearchContacts([FromQuery] string term)
    {
        var result = await _mediator.Send(new SearchContactsQuery(term));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}