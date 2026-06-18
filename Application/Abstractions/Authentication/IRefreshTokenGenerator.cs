namespace Messenger.Application.Abstractions.Authentication
{
    public interface IRefreshTokenGenerator
    {
        string Generate();
    }
}
