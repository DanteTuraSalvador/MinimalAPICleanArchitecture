namespace TestNest.Admin.SharedLibrary.Exceptions;

public sealed class SocialMediaAccountNameException : Exception, IHasErrorCode
{
    public enum ErrorCode
    {
        EmptyAccountName,
        InvalidCharacters,
        InvalidLength,
        NullSocialMediaAccountName
    }

    private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
        {
            { ErrorCode.EmptyAccountName, "Account name cannot be empty." },
            { ErrorCode.InvalidCharacters, "Account name contains invalid characters. Only letters, numbers, underscores, and periods are allowed." },
            { ErrorCode.InvalidLength, "Account name must be between 3 and 50 characters long." },
            { ErrorCode.NullSocialMediaAccountName, "Account name cannot be null." }
        };

    public ErrorCode CodeEnum { get; }

    public string Code => CodeEnum.ToString();
    public override string Message => ErrorMessages[CodeEnum];

    // Constructors
    private SocialMediaAccountNameException(ErrorCode code)
        : base(ErrorMessages[code])
    {
        CodeEnum = code;
    }

    private SocialMediaAccountNameException(ErrorCode code, Exception innerException)
        : base(ErrorMessages[code], innerException)
    {
        CodeEnum = code;
    }

    // Static helper methods for each error case
    public static SocialMediaAccountNameException EmptyAccountName()
        => new SocialMediaAccountNameException(ErrorCode.EmptyAccountName);

    public static SocialMediaAccountNameException InvalidCharacters()
        => new SocialMediaAccountNameException(ErrorCode.InvalidCharacters);

    public static SocialMediaAccountNameException InvalidLength()
        => new SocialMediaAccountNameException(ErrorCode.InvalidLength);

    public static SocialMediaAccountNameException NullSocialMediaAccountName()
        => new SocialMediaAccountNameException(ErrorCode.NullSocialMediaAccountName);
}