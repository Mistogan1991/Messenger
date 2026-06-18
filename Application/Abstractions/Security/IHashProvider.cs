namespace Messenger.Application.Abstractions.Security;

public interface IHashProvider
{
    string Hash(string value);
}
