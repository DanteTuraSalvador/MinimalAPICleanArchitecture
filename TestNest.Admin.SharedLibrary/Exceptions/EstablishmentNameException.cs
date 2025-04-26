namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EstablishmentNameException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyName,
        InvalidCharacters,
        InvalidLength,
        NullEstablishmentName
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.EmptyName, "Establishment name cannot be empty." },
        { ErrorCode.InvalidCharacters, "Establishment name contains invalid characters." },
        { ErrorCode.InvalidLength, "Establishment name must be between 3 and 50 characters." },
        { ErrorCode.NullEstablishmentName, "Establishment name cannot be null." }
    };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    // Constructors
    private EstablishmentNameException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private EstablishmentNameException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    // Static helper methods
    public static EstablishmentNameException EmptyName()
        => new(ErrorCode.EmptyName);

    public static EstablishmentNameException InvalidCharacters()
        => new(ErrorCode.InvalidCharacters);

    public static EstablishmentNameException InvalidLength()
        => new(ErrorCode.InvalidLength);

    public static EstablishmentNameException NullEstablishmentName()
        => new(ErrorCode.NullEstablishmentName);
}