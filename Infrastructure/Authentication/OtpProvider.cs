using Messenger.Application.Abstractions.Authentication;
using System.Security.Cryptography;

namespace Messenger.Infrastructure.Authentication;

public sealed class OtpProvider : IOtpProvider
{
    public string Generate(int length)
    {
        var min = (int)Math.Pow(10, length - 1);

        var max = (int)Math.Pow(10, length) - 1;

        return RandomNumberGenerator
            .GetInt32(min, max)
            .ToString();
    }

    public Task SendAsync(string phoneNumber, string code, CancellationToken ct)
    {
        Console.WriteLine(
            $"OTP => {phoneNumber} : {code}");

        return Task.CompletedTask;
    }
}
