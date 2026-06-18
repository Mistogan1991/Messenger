using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Messages.Commands.AddAttachment;

public sealed record AddAttachmentCommand(
    Guid MessageId,
    Guid FileId,
    string? Caption
) : IAppRequest;
