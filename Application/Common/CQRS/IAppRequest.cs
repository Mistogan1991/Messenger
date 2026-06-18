using MediatR;
using Messenger.Application.Common.Models;

namespace Messenger.Application.Common.CQRS;

public interface IAppRequest : IRequest<Result>
{
}
public interface IAppRequest<TRes> : IRequest<Result<TRes>>
{
}
