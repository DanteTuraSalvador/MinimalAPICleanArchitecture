namespace TestNest.Admin.SharedLibrary.Exceptions;

public class IdValidationException : Exception
{
    public string ErrorCode { get; }

    public IdValidationException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public IdValidationException(string message, string errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    // Static factory methods for specific validation errors
    public static IdValidationException InvalidGuidFormat()
    {
        return new IdValidationException("The provided ID is not a valid GUID format.", "InvalidGuidFormat");
    }

    public static IdValidationException NullOrEmptyId()
    {
        return new IdValidationException("The ID cannot be null or empty.", "NullOrEmptyId");
    }

    // Add more static factory methods as needed for other validation errors
}