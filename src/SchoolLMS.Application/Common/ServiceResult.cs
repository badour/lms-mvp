namespace SchoolLMS.Application.Common;

public class ServiceResult
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static ServiceResult Success() => new() { Succeeded = true };
    public static ServiceResult Failure(string error) => new() { Succeeded = false, Error = error, Errors = [error] };
    public static ServiceResult Failure(IEnumerable<string> errors)
    {
        var list = errors.ToList();
        return new ServiceResult { Succeeded = false, Error = list.FirstOrDefault(), Errors = list };
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; init; }

    public static ServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static ServiceResult<T> Failure(string error) => new() { Succeeded = false, Error = error, Errors = [error] };
    public new static ServiceResult<T> Failure(IEnumerable<string> errors)
    {
        var list = errors.ToList();
        return new ServiceResult<T> { Succeeded = false, Error = list.FirstOrDefault(), Errors = list };
    }
}
