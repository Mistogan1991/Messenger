using Messenger.Domain.Aggregates.Users;

namespace Messenger.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string Generate(User user, Guid sessionId);
}
