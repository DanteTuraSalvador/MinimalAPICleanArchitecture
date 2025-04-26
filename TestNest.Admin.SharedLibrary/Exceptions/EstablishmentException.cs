namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class EstablishmentException : Exception
{
    public enum ErrorCode
    {
        NotFound,
        DuplicateResource,
        InvalidId,
        Validation,
        AddressNotFound,
        AddressIsNull
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.NotFound, "Establishment not found." },
        { ErrorCode.DuplicateResource, "An establishment with this name already exists." },
        { ErrorCode.InvalidId, "Invalid establishment ID format." },
        { ErrorCode.Validation, "Validation Error" },
        { ErrorCode.AddressNotFound, "Establishment address not found." },
        { ErrorCode.AddressIsNull, "Establishment address cannot be null." }
    };

    public ErrorCode Code { get; }

    private EstablishmentException(ErrorCode code) : base(ErrorMessages[code])
    {
        Code = code;
    }

    public static EstablishmentException NotFound() => new(ErrorCode.NotFound);

    public static EstablishmentException DuplicateResource() => new(ErrorCode.DuplicateResource);

    public static EstablishmentException InvalidId() => new(ErrorCode.InvalidId);

    public static EstablishmentException Validation() => new(ErrorCode.Validation);

    public static EstablishmentException Validation(string message)
    {
        var exception = new EstablishmentException(ErrorCode.Validation);
        return new EstablishmentException(exception.Code) { Data = { { "ValidationMessage", message } } };
    }

    public static EstablishmentException AddressNotFound() => new(ErrorCode.AddressNotFound);

    public static EstablishmentException AddressIsNull() => new(ErrorCode.AddressIsNull);
}
