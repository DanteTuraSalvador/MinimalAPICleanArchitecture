namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EmployeeNumberException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyEmployeeNumber,
        InvalidEmployeeNumberFormat,
        LengthOutOfRangeEmployeeNumber,
        NullEmployeeNumber,
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
        {
            { ErrorCode.EmptyEmployeeNumber, "Employee number cannot be empty." },
            { ErrorCode.InvalidEmployeeNumberFormat, "Employee number can only contain letters, numbers and hyphens." },
            { ErrorCode.LengthOutOfRangeEmployeeNumber, "Employee number must be between 3 and 10 characters long." },
            { ErrorCode.NullEmployeeNumber, "Employee number cannot be null." }
        };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    private EmployeeNumberException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private EmployeeNumberException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    // Static helper methods for each error case
    public static EmployeeNumberException EmptyEmployeeNumber()
        => new EmployeeNumberException(ErrorCode.EmptyEmployeeNumber);

    public static EmployeeNumberException InvalidEmployeeNumberFormat()
        => new EmployeeNumberException(ErrorCode.InvalidEmployeeNumberFormat);

    public static EmployeeNumberException LengthOutOfRangeEmployeeNumber()
        => new EmployeeNumberException(ErrorCode.LengthOutOfRangeEmployeeNumber);

    public static EmployeeNumberException NullEmployeeNumber()
       => new EmployeeNumberException(ErrorCode.NullEmployeeNumber);
}