namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EmployeeStatusException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        InvalidStatusTransition,
        UnknownStatusId,
        UnknownStatusName,
        NullStatus
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.InvalidStatusTransition, "Invalid transition between employee statuses." },
        { ErrorCode.UnknownStatusId, "No employee status found for the specified ID." },
        { ErrorCode.UnknownStatusName, "No employee status found for the specified name." },
        { ErrorCode.NullStatus, "Employee status cannot be null." }
    };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    private EmployeeStatusException(ErrorCode code)
      : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    // Static factory methods
    public static EmployeeStatusException InvalidStatusTransition(EmployeeStatus current, EmployeeStatus next)
        => new(ErrorCode.InvalidStatusTransition);

    public static EmployeeStatusException UnknownStatusId(int id)
        => new(ErrorCode.UnknownStatusId);

    public static EmployeeStatusException UnknownStatusName(string name)
        => new(ErrorCode.UnknownStatusName);

    public static EmployeeStatusException NullStatus()
        => new(ErrorCode.NullStatus);
}