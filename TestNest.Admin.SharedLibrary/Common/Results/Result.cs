using Microsoft.AspNetCore.Mvc;

namespace TestNest.Admin.SharedLibrary.Common.Results;

public sealed class Result
{
    public bool IsSuccess { get; }
    public ErrorType ErrorType { get; }
    public IReadOnlyList<Error> Errors { get; }

    private Result(bool isSuccess, ErrorType errorType, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        ErrorType = errorType;
        Errors = errors ?? Array.Empty<Error>();
    }

    public static Result Success() => new(true, ErrorType.None, Array.Empty<Error>());

    public static Result Failure(ErrorType errorType, Error error)
    {
        if (errorType == ErrorType.None)
            throw ResultException.InvalidErrorType(typeof(Result));

        ResultException.ValidateErrorCodeAndMessage(error.Code, error.Message);

        return new Result(false, errorType, new[] { error });
    }

    public static Result Failure(ErrorType errorType, IEnumerable<Error> errors)
    {
        if (errorType == ErrorType.None)
            throw ResultException.InvalidErrorType(typeof(Result));

        var errorList = errors?.Where(e => e != null).ToList()
                        ?? throw new ArgumentNullException(nameof(errors));

        if (!errorList.Any())
            throw ResultException.EmptyErrors(typeof(Result));

        foreach (var error in errorList)
        {
            ResultException.ValidateErrorCodeAndMessage(error.Code, error.Message);
        }

        return new Result(false, errorType, errorList);
    }

    public static Result Failure(ErrorType errorType, string code, string message) =>
        Failure(errorType, new Error(code, message));

    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(r => !r.IsSuccess).ToList();
        return failures.Any()
            ? Failure(ErrorType.Aggregate, failures.SelectMany(r => r.Errors))
            : Success();
    }

    public Result<T> ToResult<T>(T value) =>
        IsSuccess ? Result<T>.Success(value) : Result<T>.Failure(ErrorType, Errors);

    public Result Bind(Func<Result> bind) =>
        IsSuccess ? bind() : this;

    public Result<T> Bind<T>(Func<Result<T>> bind) =>
        IsSuccess ? bind() : Result<T>.Failure(ErrorType, Errors);

    public Result Map(Action map)
    {
        if (IsSuccess)
            map();
        return this;
    }

    public Result<T> Map<T>(Func<T> map) =>
        IsSuccess ? Result<T>.Success(map()) : Result<T>.Failure(ErrorType, Errors);

    public async Task<Result> BindAsync(Func<Task<Result>> bindAsync) =>
        IsSuccess ? await bindAsync() : this;

    public async Task<Result<T>> BindAsync<T>(Func<Task<Result<T>>> bindAsync) =>
        IsSuccess ? await bindAsync() : Result<T>.Failure(ErrorType, Errors);

    public async Task<Result> MapAsync(Func<Task> mapAsync)
    {
        if (IsSuccess)
            await mapAsync();
        return this;
    }

    public async Task<Result<T>> MapAsync<T>(Func<Task<T>> mapAsync) =>
        IsSuccess ? Result<T>.Success(await mapAsync()) : Result<T>.Failure(ErrorType, Errors);

    public void Deconstruct(out bool isSuccess, out ErrorType errorType, out IReadOnlyList<Error> errors)
    {
        isSuccess = IsSuccess;
        errorType = ErrorType;
        errors = Errors;
    }

    public static Result Failure(Exception exception)
    {
        var error = exception switch
        {
            IHasErrorCode exWithCode => new Error(exWithCode.Code, exWithCode.Message),
            _ => new Error(exception.GetType().Name, exception.Message)
        };

        return Failure(ErrorType.Validation, error);
    }

    public IActionResult Match(
       Func<IActionResult> onSuccess,
       Func<ErrorType, IEnumerable<Error>, IActionResult> onFailure)
    {
        return IsSuccess
            ? onSuccess()
            : onFailure(ErrorType, Errors);
    }
}

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public ErrorType ErrorType { get; }
    public IReadOnlyList<Error> Errors { get; }

    private Result(bool isSuccess, ErrorType errorType, T? value, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        ErrorType = errorType;
        Value = value;
        Errors = errors ?? Array.Empty<Error>();
    }

    public static Result<T> Success(T value) =>
        value is null
            ? throw ResultException.NullValue(typeof(T))
            : new Result<T>(true, ErrorType.None, value, Array.Empty<Error>());

    public static Result<T> Failure(ErrorType errorType, Error error)
    {
        if (errorType == ErrorType.None)
            throw ResultException.InvalidErrorType(typeof(Result<T>));

        ResultException.ValidateErrorCodeAndMessage(error.Code, error.Message);

        return new Result<T>(false, errorType, default, new[] { error });
    }

    public static Result<T> Failure(ErrorType errorType, IEnumerable<Error> errors)
    {
        if (errorType == ErrorType.None)
            throw ResultException.InvalidErrorType(typeof(Result<T>));

        var errorList = errors?.Where(e => e != null).ToList()
                        ?? throw new ArgumentNullException(nameof(errors));

        if (!errorList.Any())
            throw ResultException.EmptyErrors(typeof(Result<T>));

        foreach (var error in errorList)
        {
            ResultException.ValidateErrorCodeAndMessage(error.Code, error.Message);
        }

        return new Result<T>(false, errorType, default, errorList);
    }

    public static Result<T> Failure(ErrorType errorType, string code, string message) =>
        Failure(errorType, new Error(code, message));

    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> bind) =>
        IsSuccess ? bind(Value!) : Result<TNew>.Failure(ErrorType, Errors);

    public Result<TNew> Map<TNew>(Func<T, TNew> map) =>
        IsSuccess ? Result<TNew>.Success(map(Value!)) : Result<TNew>.Failure(ErrorType, Errors);

    public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Task<Result<TNew>>> bindAsync) =>
        IsSuccess ? await bindAsync(Value!) : Result<TNew>.Failure(ErrorType, Errors);

    public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapAsync) =>
        IsSuccess ? Result<TNew>.Success(await mapAsync(Value!)) : Result<TNew>.Failure(ErrorType, Errors);

    public T EnsureSuccess() =>
      IsSuccess ? Value! : throw ResultException.Failure(typeof(T), Errors.Select(e => e.Message));

    public bool TryGetValue(out T? value, out IReadOnlyList<Error> errors)
    {
        value = IsSuccess ? Value : default;
        errors = Errors;
        return IsSuccess;
    }

    public void Deconstruct(out bool isSuccess, out T? value, out ErrorType errorType, out IReadOnlyList<Error> errors)
    {
        isSuccess = IsSuccess;
        value = Value;
        errorType = ErrorType;
        errors = Errors;
    }

    public static implicit operator Result<T>(T value) => Success(value);

    public Result ToResult()
    {
        return IsSuccess ? Result.Success() : Result.Failure(ErrorType, Errors);
    }

    public IActionResult Match(
      Func<T, IActionResult> onSuccess,
      Func<ErrorType, IEnumerable<Error>, IActionResult> onFailure)
    {
        return IsSuccess
            ? onSuccess(Value!)
            : onFailure(ErrorType, Errors);
    }
}
