namespace Messenger.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(IReadOnlyCollection<string> errors)
    {
        Errors = errors;
    }
}
