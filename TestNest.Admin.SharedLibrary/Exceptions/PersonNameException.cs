namespace TestNest.Admin.SharedLibrary.Exceptions;

public class PersonNameException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyFirstName,
        EmptyLastName,
        InvalidCharacters,
        InvalidLength,
        EmptyPerson,
        NullPersonName
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
        {
            { ErrorCode.EmptyFirstName, "First name cannot be empty." },
            { ErrorCode.EmptyLastName, "Last name cannot be empty." },
            { ErrorCode.InvalidCharacters, "Names can only contain letters, apostrophes, and hyphens." },
            { ErrorCode.InvalidLength, "Names must be between 2 and 100 characters long." },
            { ErrorCode.EmptyPerson, "Person name is empty" },
            { ErrorCode.NullPersonName, "Person name is invalid" }
        };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    // Constructors
    private PersonNameException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private PersonNameException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    // Helper methods to return error messages
    public static PersonNameException EmptyFirstName() => new PersonNameException(ErrorCode.EmptyFirstName);

    public static PersonNameException EmptyLastName() => new PersonNameException(ErrorCode.EmptyLastName);

    public static PersonNameException InvalidCharacters() => new PersonNameException(ErrorCode.InvalidCharacters);

    public static PersonNameException InvalidLength() => new PersonNameException(ErrorCode.InvalidLength);

    public static PersonNameException EmptyPerson() => new PersonNameException(ErrorCode.EmptyPerson);

    public static PersonNameException NullPersonName() => new PersonNameException(ErrorCode.NullPersonName);
}