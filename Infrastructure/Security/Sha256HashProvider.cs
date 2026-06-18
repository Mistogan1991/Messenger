using Messenger.Application.Abstractions.Security;
using System.Security.Cryptography;
using System.Text;

namespace Messenger.Infrastructure.Security;

public sealed class Sha256HashProvider : IHashProvider
{
    public string Hash(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);

        var hashBytes = SHA256.HashData(bytes);

        return Convert.ToHexString(hashBytes);
    }
}