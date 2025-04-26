namespace TestNest.Admin.SharedLibrary.Exceptions;

public class ResultException : Exception
{
    public Type ResultType { get; }
    public IReadOnlyList<string> Errors { get; }

    private ResultException(string message, Type resultType, IEnumerable<string> errors)
        : base(message)
    {
        ResultType = resultType;
        Errors = errors.ToList();
    }

    public static void ValidateErrorCodeAndMessage(string code, string message)
    {
        if (string.IsNullOrEmpty(code))
        {
            throw new ResultException("Error code cannot be null or empty.", typeof(Error), new[] { "Error code cannot be null or empty." });
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ResultException("Error message cannot be null or empty.", typeof(Error), new[] { "Error message cannot be null or empty." });
        }
    }

    public static ResultException NullValue(Type resultType) =>
        new($"Result<{resultType.Name}> cannot have a null value.", resultType, new[] { "Null value is not allowed." });

    public static ResultException EmptyErrors(Type resultType) =>
        new($"Result<{resultType.Name}> must contain at least one error message.", resultType, new[] { "Error list cannot be empty." });

    public static ResultException Failure(Type resultType, IEnumerable<string> errors) =>
        new($"Result<{resultType.Name}> operation failed.", resultType, errors);

    public static ResultException InvalidErrorType(Type resultType) =>
        new($"Failure must have a valid error type. Cannot use ErrorType.None in Result<{resultType.Name}>.", resultType, new[] { "Invalid error type." });

    public static ResultException NoValidResults(Type resultType) =>
        new($"No valid success results found in Result<{resultType.Name}>.", resultType, new[] { "No valid results available." });

    public override string ToString() => $"ResultException: {Message} (Errors: {string.Join(", ", Errors)})";
}