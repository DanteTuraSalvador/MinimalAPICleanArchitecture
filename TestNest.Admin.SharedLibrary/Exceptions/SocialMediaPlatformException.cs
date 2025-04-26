namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class SocialMediaPlatformException : Exception
{
    public enum ErrorCode
    {
        NotFound,
        DuplicateResource,
        InvalidId,
        Validation
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
    {
        { ErrorCode.NotFound, "Social media platform not found." },
        { ErrorCode.DuplicateResource, "A social media platform with this name already exists." },
        { ErrorCode.InvalidId, "Invalid social media ID format." },
        { ErrorCode.Validation, "Validation Error" }
    };

    public ErrorCode Code { get; }

    private SocialMediaPlatformException(ErrorCode code) : base(ErrorMessages[code])
    {
        Code = code;
    }

    public static SocialMediaPlatformException NotFound() => new(ErrorCode.NotFound);

    public static SocialMediaPlatformException DuplicateResource() => new(ErrorCode.DuplicateResource);

    public static SocialMediaPlatformException InvalidId() => new(ErrorCode.InvalidId);

    public static SocialMediaPlatformException Validation() => new(ErrorCode.Validation);

    public static SocialMediaPlatformException Validation(string message)
    {
        var exception = new SocialMediaPlatformException(ErrorCode.Validation);
        return new SocialMediaPlatformException(exception.Code) { Data = { { "ValidationMessage", message } } };
    }
}