namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EmployeeRoleException : Exception
{
    public enum ErrorCode
    {
        NullEmployeeRole,
        NotFound,
        DuplicateResource,
        InvalidId,
        Validation
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.NullEmployeeRole, "Employee role cannot be null or empty." },
        { ErrorCode.NotFound, "Employee role not found." },
        { ErrorCode.DuplicateResource, "A employee role with this name already exists." },
        { ErrorCode.InvalidId, "Invalid employee role ID format." },
        { ErrorCode.Validation, "Validation Error" }
    };

    public ErrorCode Code { get; }

    private EmployeeRoleException(ErrorCode code) : base(ErrorMessages[code])
    {
        Code = code;
    }

    public static EmployeeRoleException NullEmployeeRole() => new(ErrorCode.NullEmployeeRole);

    public static EmployeeRoleException NotFound() => new(ErrorCode.NotFound);

    public static EmployeeRoleException DuplicateResource() => new(ErrorCode.DuplicateResource);

    public static EmployeeRoleException InvalidId() => new(ErrorCode.InvalidId);

    public static EmployeeRoleException Validation() => new(ErrorCode.Validation);

    public static EmployeeRoleException Validation(string message)
    {
        var exception = new EmployeeRoleException(ErrorCode.Validation);
        return new EmployeeRoleException(exception.Code) { Data = { { "ValidationMessage", message } } };
    }
}