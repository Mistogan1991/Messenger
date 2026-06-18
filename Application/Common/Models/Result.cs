namespace Messenger.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public string? ErrorCode { get; init; }
    public IReadOnlyCollection<string> Errors { get; init; } = [];

    public static Result Success()
        => new() { IsSuccess = true, Errors = [] };

    public static Result Failure(IEnumerable<string> errors)
        => new() { IsSuccess = false, Errors = [.. errors] };
}

public class Result<T> : Result
{
    public T Data { get; set; } = default!;

    public static Result<T> Success(T data)
        => new() { IsSuccess = true, Data = data, Errors = [] };

    public new static Result<T> Failure(IEnumerable<string> errors)
        => new() { IsSuccess = false, Errors = [.. errors] };
}