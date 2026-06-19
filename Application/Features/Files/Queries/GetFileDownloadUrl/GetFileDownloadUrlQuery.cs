using Messenger.Application.Common.CQRS;

namespace Messenger.Application.Features.Files.Queries.GetFileDownloadUrl;

public sealed record GetFileDownloadUrlQuery(Guid FileId) : IAppRequest<string>;
