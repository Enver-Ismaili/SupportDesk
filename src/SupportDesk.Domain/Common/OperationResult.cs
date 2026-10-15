namespace SupportDesk.Domain.Common;

public sealed class OperationResult
{
    public bool Success { get; }
    public string Message { get; }
    public string? Code { get; }

    private OperationResult(bool success, string message, string? code)
    {
        Success = success;
        Message = message;
        Code = code;
    }

    public static OperationResult Ok(string message = "OK") => new(true, message, null);

    public static OperationResult Fail(string code, string message) => new(false, message, code);
}

public sealed class OperationResult<T>
{
    public bool Success { get; }
    public string Message { get; }
    public string? Code { get; }
    public T? Value { get; }

    private OperationResult(bool success, string message, string? code, T? value)
    {
        Success = success;
        Message = message;
        Code = code;
        Value = value;
    }

    public static OperationResult<T> Ok(T value, string message = "OK") => new(true, message, null, value);

    public static OperationResult<T> Fail(string code, string message) => new(false, message, code, default);
}
