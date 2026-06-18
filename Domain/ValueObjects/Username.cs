using Messenger.Domain.Common;

namespace Messenger.Domain.ValueObjects;

public sealed class Username : ValueObject
{
    public string Value { get; }

    private Username(string value)
    {
        Value = value;
    }

    public static Username Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username required");

        if (value.Length < 3 || value.Length > 30)
            throw new ArgumentException("Invalid username");

        return new Username(value.ToLower());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}