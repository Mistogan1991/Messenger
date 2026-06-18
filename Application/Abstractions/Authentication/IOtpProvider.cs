namespace Messenger.Application.Abstractions.Authentication;

public interface IOtpProvider
{
    string Generate(int length);

    Task SendAsync(string phoneNumber, string code, CancellationToken ct);
}
