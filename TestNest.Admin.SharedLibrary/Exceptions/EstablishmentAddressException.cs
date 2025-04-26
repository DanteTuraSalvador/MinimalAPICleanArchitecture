namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EstablishmentAddressException : Exception
{
    public enum ErrorCode
    {
        NotFound,
        InvalidId,
        Validation
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.NotFound, "Establishment address not found." },
        { ErrorCode.InvalidId, "Invalid establishment address ID format." },
        { ErrorCode.Validation, "Validation Error" }
    };

    public ErrorCode Code { get; }

    private EstablishmentAddressException(ErrorCode code) : base(ErrorMessages[code])
    {
        Code = code;
    }

    public static EstablishmentAddressException NotFound() => new(ErrorCode.NotFound);

    public static EstablishmentAddressException InvalidId() => new(ErrorCode.InvalidId);

    public static EstablishmentAddressException Validation() => new(ErrorCode.Validation);

    public static EstablishmentAddressException Validation(string message)
    {
        var exception = new EstablishmentAddressException(ErrorCode.Validation);
        return new EstablishmentAddressException(exception.Code) { Data = { { "ValidationMessage", message } } };
    }
}