namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class RoleNameException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyRoleName,
        InvalidRoleNameCharacters,
        InvalidRoleNameLength,
        NullRoleName
    }

    public static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
        {
            { ErrorCode.EmptyRoleName, "Role name cannot be empty." },
            { ErrorCode.InvalidRoleNameCharacters, "Role name contains invalid characters." },
            { ErrorCode.InvalidRoleNameLength, "Role name must be between 3 and 100 characters." },
            { ErrorCode.NullRoleName, "Role name cannot be null." }
        };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    private RoleNameException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private RoleNameException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    public static RoleNameException EmptyRoleName()
        => new(ErrorCode.EmptyRoleName);

    public static RoleNameException InvalidRoleNameCharacters()
        => new(ErrorCode.InvalidRoleNameCharacters);

    public static RoleNameException InvalidRoleNameLength()
        => new(ErrorCode.InvalidRoleNameLength);

    public static RoleNameException NullRoleName()
        => new(ErrorCode.NullRoleName);
}