using System.Text.RegularExpressions;

namespace TestNest.Admin.SharedLibrary.Common.Guards;

public static class Guard
{
    public static Result Against(Func<bool> validation, Func<Exception> exceptionFactory)
    {
        try
        {
            return validation()
                ? Result.Success()
                : Result.Failure(exceptionFactory());
        }
        catch (Exception ex)
        {
            return Result.Failure(new Exception("Validation failed", ex));
        }
    }

    public static Result AgainstNull<T>(T value, Func<Exception> exceptionFactory) where T : class
        => Against(() => value != null, exceptionFactory);

    public static Result AgainstNullOrWhiteSpace(string value, Func<Exception> exceptionFactory)
        => Against(() => !string.IsNullOrWhiteSpace(value), exceptionFactory);

    public static Result AgainstLength(string value, int requiredLength, Func<Exception> exceptionFactory)
        => Against(() => value.Length == requiredLength, exceptionFactory);

    public static Result AgainstRegex(string value, string pattern, Func<Exception> exceptionFactory)
        => Against(() => Regex.IsMatch(value, pattern), exceptionFactory);

    public static Result AgainstInvalidInput<T>(T value, Func<T, bool> validation, Func<Exception> exceptionFactory)
        => Against(() => validation(value), exceptionFactory);

    public static Result AgainstRange(int value, int min, int max, Func<Exception> exceptionFactory)
        => Against(() => value >= min && value <= max, exceptionFactory);

    public static Result AgainstNegative(decimal value, Func<Exception> exceptionFactory)
       => Against(() => value >= 0, exceptionFactory);

    public static Result AgainstNonPositive(decimal value, Func<Exception> exceptionFactory)
        => Against(() => value > 0, exceptionFactory);

    public static Result AgainstEmptyCollection<T>(IEnumerable<T> collection, Func<Exception> exceptionFactory)
        => Against(() => collection != null && collection.Any(), exceptionFactory);

    public static Result AgainstPastDate(DateTime date, Func<Exception> exceptionFactory)
        => Against(() => date >= DateTime.UtcNow, exceptionFactory);

    public static Result AgainstFutureDate(DateTime date, Func<Exception> exceptionFactory)
        => Against(() => date <= DateTime.UtcNow, exceptionFactory);

    public static Result AgainstCondition(bool invalidCondition, Func<Exception> exceptionFactory)
        => Against(() => !invalidCondition, exceptionFactory);

    public static Result Aggregate(params Result[] results)
        => Result.Combine(results);
}
