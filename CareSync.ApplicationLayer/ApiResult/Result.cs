using System.Net;
using System.Text.Json.Serialization;

namespace CareSync.ApplicationLayer.ApiResult;

public class Result<T>
{
    public HttpStatusCode StatusCode { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => _exception != null || !IsSuccess;

    public T? Data { get; }
    private readonly string? _error;

    [JsonIgnore]
    private readonly Exception? _exception;

    public object? Error =>
        _exception == null ? null : new
        {
            Type = _exception.GetType().Name,
            Message = _exception.Message,
            StackTrace = _exception.StackTrace,
            InnerMessage = _exception.InnerException?.Message
        };

    private Result(bool isSuccess, T? value, string? error, Exception? exception, HttpStatusCode statusCode)
    {
        (IsSuccess, StatusCode, Data, _error, _exception) = (isSuccess, statusCode, value, error, exception);
    }

    public static Result<T> Success(T value, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new Result<T>(true, value, null, null, statusCode);

    public static Result<T> Failure(T value, string error = "", HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new Result<T>(false, value, error, null, statusCode);

    public static Result<T> Exception(Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        => new Result<T>(false, default, ex.Message, ex, statusCode);

    public T? GetValueOrDefault() => Data;
    public string? GetError() => _error;
    public Exception? GetException() => _exception;
}
