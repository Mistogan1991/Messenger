using Messenger.Domain.Common;

namespace Messenger.Domain.ValueObjects;

public sealed class MessageContent : ValueObject
{
    public string Value { get; }

    private MessageContent(string value)
    {
        Value = value;
    }

    public static MessageContent Create(string value)
    {
        if (value.Length > 4000)
            throw new ArgumentException("Message too long");

        return new MessageContent(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}