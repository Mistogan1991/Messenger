using MediatR;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Common.CQRS;

public interface IAppRequestHandler<in TCommandQuery>
    : IRequestHandler<TCommandQuery, Result> where TCommandQuery : IRequest<Result>
{
}

public interface IAppRequestHandler<in TCommandQuery, TRes>
    : IRequestHandler<TCommandQuery, Result<TRes>> where TCommandQuery : IRequest<Result<TRes>>
{
}
