using Messenger.Application.Abstractions.Authentication;
using System.Security.Cryptography;

namespace Messenger.Infrastructure.Authentication;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string Generate()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
