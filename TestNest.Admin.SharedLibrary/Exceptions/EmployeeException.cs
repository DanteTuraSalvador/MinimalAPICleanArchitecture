namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EmployeeException : Exception
{
    public enum ErrorCode
    {
        NotFound,
        DuplicateResource,
        InvalidId,
        Validation,
        InvalidEmployeeRoleId,
        InvalidEstablishmentId
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.NotFound, "Employee not found." },
        { ErrorCode.DuplicateResource, "An employee with this details already exists." },
        { ErrorCode.InvalidId, "Invalid employee ID format." },
        { ErrorCode.Validation, "Validation Error" },
        { ErrorCode.InvalidEmployeeRoleId, "Invalid Employee Role Id" },
        { ErrorCode.InvalidEstablishmentId, "Invalid Establishment Id"}
    };

    public ErrorCode Code { get; }
    public string Message { get; }

    public const string DuplicateEmployeeErrorMessage = "An employee with the same number, name, email, and establishment already exists.";

    private EmployeeException(ErrorCode code, string message) : base(message)
    {
        Code = code;
        Message = message;
    }

    public static EmployeeException NotFound() => new(ErrorCode.NotFound, ErrorMessages[ErrorCode.NotFound]);

    public static EmployeeException DuplicateResource => new(ErrorCode.DuplicateResource, ErrorMessages[ErrorCode.DuplicateResource]);

    public static EmployeeException InvalidId() => new(ErrorCode.InvalidId, ErrorMessages[ErrorCode.InvalidId]);

    public static EmployeeException Validation() => new(ErrorCode.Validation, ErrorMessages[ErrorCode.Validation]);

    public static EmployeeException InvalidEmployeeRoleId => new(ErrorCode.InvalidEmployeeRoleId, ErrorMessages[ErrorCode.InvalidEmployeeRoleId]);
    public static EmployeeException InvalidEstablishmentId => new(ErrorCode.InvalidEstablishmentId, ErrorMessages[ErrorCode.InvalidEstablishmentId]);

    public static EmployeeException Validation(string message)
        => new EmployeeException(ErrorCode.Validation, message);
}
