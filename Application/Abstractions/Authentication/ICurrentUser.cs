namespace Messenger.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid SessionId { get; }
    string PhoneNumber { get; }
    bool IsAuthenticated { get; }
}
