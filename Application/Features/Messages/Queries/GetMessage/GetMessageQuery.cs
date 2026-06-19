using Messenger.Application.Common.CQRS;
using Messenger.Application.Features.Messages.Dtos;

namespace Messenger.Application.Features.Messages.Queries.GetMessage;

public sealed record GetMessageQuery(Guid MessageId) : IAppRequest<MessageDto>;
