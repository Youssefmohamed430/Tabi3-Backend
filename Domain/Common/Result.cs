namespace Domain.Common;

/// <summary>
/// Represents the outcome of an operation that does not return data.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string msg, Error error)
    {
        if (isSuccess == !error.IsNone)
        {
            throw new ArgumentException("The result status and error must agree.", nameof(error));
        }

        IsSuccess = isSuccess;
        Msg = msg;
        Error = error;
    }

    public bool IsSuccess { get; }
    public string Msg { get; }
    public Error Error { get; }

    public static Result Success(string msg = "") => new(true, msg, Error.None);

    public static Result Failure(Error error) => new(false, error.Description, error);

    public static Result Failure(string msg) => Failure(new Error("General.Failure", msg));

    public static Result<T> Success<T>(T data, string msg = "") => new(data, true, msg, Error.None);

    public static Result<T> Failure<T>(Error error) => new(default, false, error.Description, error);

    public static Result<T> Failure<T>(string msg) => Failure<T>(new Error("General.Failure", msg));
}

/// <summary>
/// Represents the outcome of an operation that returns one value or a collection.
/// </summary>
public sealed class Result<T> : Result
{
    internal Result(T? data, bool isSuccess, string msg, Error error)
        : base(isSuccess, msg, error)
    {
        Data = data;
    }

    public T? Data { get; }
}
