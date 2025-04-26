namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class PhoneNumberException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyPhoneNumber,
        InvalidFormat,
        NullPhoneNumber
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
        {
            { ErrorCode.EmptyPhoneNumber, "Phone number cannot be empty." },
            { ErrorCode.InvalidFormat, "Invalid phone number format. Use E.164 format (e.g., +1234567890)." },
            { ErrorCode.NullPhoneNumber, "Phone number cannot be null." }
        };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    // Constructors
    private PhoneNumberException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private PhoneNumberException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    // Static helper methods for each error case
    public static PhoneNumberException EmptyPhoneNumber()
        => new PhoneNumberException(ErrorCode.EmptyPhoneNumber);

    public static PhoneNumberException InvalidFormat()
        => new PhoneNumberException(ErrorCode.InvalidFormat);

    public static PhoneNumberException NullPhoneNumber()
       => new PhoneNumberException(ErrorCode.NullPhoneNumber);
}