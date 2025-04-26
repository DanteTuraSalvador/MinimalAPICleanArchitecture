namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class SocialMediaNameException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyName,
        InvalidCharacters,
        InvalidLength,
        EmptyPlatformURL,
        InvalidPlatformURLFormat,
        NullSocialMediaName
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
        {
            { ErrorCode.EmptyName, "Social media name cannot be empty." },
            { ErrorCode.InvalidCharacters, "Social media name contains invalid characters. Only letters, numbers, underscores, and periods are allowed." },
            { ErrorCode.InvalidLength, "Social media name must be between 3 and 50 characters long." },
            { ErrorCode.EmptyPlatformURL, "Platform URL cannot be empty." },
            { ErrorCode.InvalidPlatformURLFormat, "Platform URL format is invalid." },
            { ErrorCode.NullSocialMediaName, "Social media account name cannot be null." }
        };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    // Constructors
    private SocialMediaNameException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private SocialMediaNameException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    public static SocialMediaNameException EmptyName() => new(ErrorCode.EmptyName);

    public static SocialMediaNameException InvalidCharacters() => new(ErrorCode.InvalidCharacters);

    public static SocialMediaNameException InvalidLength() => new(ErrorCode.InvalidLength);

    public static SocialMediaNameException EmptyPlatformURL() => new(ErrorCode.EmptyPlatformURL);

    public static SocialMediaNameException InvalidPlatformURLFormat() => new(ErrorCode.InvalidPlatformURLFormat);

    public static SocialMediaNameException NullSocialMediaName() => new(ErrorCode.NullSocialMediaName);
}