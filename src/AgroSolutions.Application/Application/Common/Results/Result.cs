using AgroSolutions.Application.Common.Notifications;

namespace AgroSolutions.Application.Common.Results;

/// <summary>
/// Result pattern for operation outcomes
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public IReadOnlyCollection<Notification> Errors { get; private set; }

    private Result(bool isSuccess, T? value, IReadOnlyCollection<Notification> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, Array.Empty<Notification>());
    }

    public static Result<T> Failure(IReadOnlyCollection<Notification> errors)
    {
        return new Result<T>(false, default, errors);
    }

    public static Result<T> Failure(string key, string message)
    {
        return new Result<T>(false, default, new[] { new Notification(key, message) });
    }
}

/// <summary>
/// Non-generic Result for operations without return value
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public IReadOnlyCollection<Notification> Errors { get; private set; }

    private Result(bool isSuccess, IReadOnlyCollection<Notification> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success()
    {
        return new Result(true, Array.Empty<Notification>());
    }

    public static Result Failure(IReadOnlyCollection<Notification> errors)
    {
        return new Result(false, errors);
    }

    public static Result Failure(string key, string message)
    {
        return new Result(false, new[] { new Notification(key, message) });
    }
}
