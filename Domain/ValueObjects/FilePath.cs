using Messenger.Domain.Common;

namespace Messenger.Domain.ValueObjects;

public sealed class FilePath : ValueObject
{
    public string Value { get; }

    private FilePath(string value)
    {
        Value = value;
    }

    public static FilePath Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("File path required");

        return new FilePath(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}