namespace SunnyBookShop.Utils;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public Dictionary<string, string> Errors { get; } = new();

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(Dictionary<string, string> errors)
    {
        IsSuccess = false;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Dictionary<string, string> errors) => new(errors);
}