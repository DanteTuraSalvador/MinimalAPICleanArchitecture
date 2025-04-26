namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EmailAddressException : Exception
{
    public enum ErrorCode
    {
        InvalidFormat,
        NullEmailAddress,
        EmptyEmailAddress,
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.InvalidFormat, "Invalid email format." },
        { ErrorCode.NullEmailAddress, "Email address canoot be null." },
        { ErrorCode.EmptyEmailAddress, "Email address cannot be empty." }
    };

    public ErrorCode Code { get; }

    public EmailAddressException(ErrorCode code) : base(ErrorMessages[code])
    {
        Code = code;
    }

    public static EmailAddressException InvalidFormat() => new EmailAddressException(ErrorCode.InvalidFormat);

    public static EmailAddressException NullEmailAddress() => new EmailAddressException(ErrorCode.NullEmailAddress);

    public static EmailAddressException EmptyEmailAddress() => new EmailAddressException(ErrorCode.EmptyEmailAddress);
}