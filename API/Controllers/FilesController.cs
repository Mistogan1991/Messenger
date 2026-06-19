using MediatR;
using Messenger.API.Contracts.Files;
using Messenger.Application.Features.Files.Commands.RequestFileUpload;
using Messenger.Application.Features.Files.Queries.GetFileDownloadUrl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.API.Controllers;

[Authorize]
[ApiController]
[Route("api/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Reserves a file record and returns a pre-signed URL to upload the bytes to.</summary>
    [HttpPost("upload-url")]
    public async Task<IActionResult> RequestUpload(RequestFileUploadRequest request)
    {
        var result = await _mediator.Send(new RequestFileUploadCommand(
            request.FileName, request.ContentType, request.Size, request.Type));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Returns a short-lived pre-signed URL to download a stored file.</summary>
    [HttpGet("{fileId:guid}/download-url")]
    public async Task<IActionResult> GetDownloadUrl(Guid fileId)
    {
        var result = await _mediator.Send(new GetFileDownloadUrlQuery(fileId));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
