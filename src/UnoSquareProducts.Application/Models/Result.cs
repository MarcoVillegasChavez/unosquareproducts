using mx.unosquare.products.application.Enums;
using System.Text.Json.Serialization;

namespace mx.unosquare.products.application.Models;

public class Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public ResultStatus Status { get; }
    public bool IsSuccess => Status == ResultStatus.Success;

    [JsonConstructor]
    private Result(T? value, string? error, ResultStatus status)
    {
        Value = value;
        Error = error;
        Status = status;
    }
    public static Result<T> Success(T value) =>
           new(value, null, ResultStatus.Success);

    public static Result<T> Fail(string error, ResultStatus status) =>
        new(default, error, status);
}


