namespace Domain.Shared;

public class Result
{
    public Result(Error error, bool isSuccess)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        Error = error;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new Result(Error.None, true);

    public static Result Failure(Error error) => new(error, false);

    public static Result<TValue> Success<TValue>(TValue value) => new(Error.None, true, value);

    public static Result<TValue> Failure<TValue>(Error error) => new(error, false, default);

    public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
