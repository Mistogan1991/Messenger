using FluentValidation;

namespace Messenger.Application.Features.Files.Commands.RequestFileUpload;

public sealed class RequestFileUploadValidator : AbstractValidator<RequestFileUploadCommand>
{
    /// <summary>Hard upper bound on a single upload (2 GiB), matching Telegram's large-file ceiling.</summary>
    public const long MaxFileSizeBytes = 2L * 1024 * 1024 * 1024;

    public RequestFileUploadValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Size).GreaterThan(0).LessThanOrEqualTo(MaxFileSizeBytes);
        RuleFor(x => x.Type).IsInEnum();
    }
}
