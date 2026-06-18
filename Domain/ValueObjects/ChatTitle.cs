using Messenger.Domain.Common;

namespace Messenger.Domain.ValueObjects;

public sealed class ChatTitle : ValueObject
{
    public string Value { get; }

    private ChatTitle(string value)
    {
        Value = value;
    }

    public static ChatTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Title required");

        if (value.Length > 100)
            throw new ArgumentException("Too long");

        return new ChatTitle(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}