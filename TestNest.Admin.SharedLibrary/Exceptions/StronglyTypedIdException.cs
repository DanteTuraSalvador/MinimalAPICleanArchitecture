namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class StronglyTypedIdException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        NullInstanceCreation,
        InvalidGuidCreation,
        InvalidFormat,
        NullId,
        Deserialization,
        NullBindingContext,
        MissingValue
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.NullInstanceCreation, "Failed to create instance of {0}. Activator returned null." },
        { ErrorCode.InvalidGuidCreation, "Invalid GUID provided for {0}." },
        { ErrorCode.InvalidFormat, "Invalid format." },
        { ErrorCode.NullId, "ID cannot be null or empty." },
        { ErrorCode.Deserialization, "Couldn't deserialize from JSON. Invalid format." },
        { ErrorCode.NullBindingContext, "The binding context cannot be null in StronglyTypedIdModelBinder." },
        { ErrorCode.MissingValue, "No value was provided for the strongly typed ID in the request." }
    };

    public string Code { get; }

    private StronglyTypedIdException(ErrorCode code, string message)
        : base(message)
    {
        Code = code.ToString();
    }

    public static StronglyTypedIdException NullInstanceCreation() =>
        new(ErrorCode.NullInstanceCreation, ErrorMessages[ErrorCode.NullInstanceCreation]);

    public static StronglyTypedIdException InvalidGuidCreation() =>
        new(ErrorCode.InvalidGuidCreation, ErrorMessages[ErrorCode.InvalidGuidCreation]);

    public static StronglyTypedIdException InvalidFormat() =>
        new(ErrorCode.InvalidFormat, ErrorMessages[ErrorCode.InvalidFormat]);

    public static StronglyTypedIdException NullId() =>
        new(ErrorCode.NullId, ErrorMessages[ErrorCode.NullId]);

    public static StronglyTypedIdException Deserialization() =>
       new(ErrorCode.Deserialization, ErrorMessages[ErrorCode.Deserialization]);

    public static StronglyTypedIdException NullBindingContext() =>
       new(ErrorCode.NullBindingContext, ErrorMessages[ErrorCode.NullBindingContext]);

    public static StronglyTypedIdException MissingValue() =>
       new(ErrorCode.MissingValue, ErrorMessages[ErrorCode.MissingValue]);
}