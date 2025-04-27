using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class SocialMediaName : ValueObject
{
    private static readonly Regex ValidNamePattern = new("^[A-Za-z0-9_.]+$", RegexOptions.Compiled);
    public string Name { get; }
    public string PlatformURL { get; }

    private SocialMediaName() => (Name, PlatformURL) = (string.Empty, string.Empty);

    private SocialMediaName(string name, string platformURL) => (Name, PlatformURL) = (name, platformURL);

    private static readonly Lazy<SocialMediaName> _empty = new(() => new SocialMediaName());

    public static SocialMediaName Empty() => _empty.Value;

    public static Result<SocialMediaName> Create(string name, string platformURL)
    {
        Result validationResult = ValidateSocialMediaName(name, platformURL);
        return validationResult.IsSuccess
            ? Result<SocialMediaName>.Success(new SocialMediaName(name, platformURL))
            : Result<SocialMediaName>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<SocialMediaName> WithNamePlatform(string newName, string newPlatformURL)
        => Create(newName, newPlatformURL);

    public Result<SocialMediaName> WithName(string newName)
        => Create(newName, PlatformURL);

    public Result<SocialMediaName> WithPlatformURL(string newPlatformURL)
        => Create(Name, newPlatformURL);

    public static Result<SocialMediaName> TryParse(string name, string platformURL)
    {
        Result validationResult = ValidateSocialMediaName(name, platformURL);
        return validationResult.IsSuccess
            ? Result<SocialMediaName>.Success(new SocialMediaName(name, platformURL))
            : Result<SocialMediaName>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    private static Result ValidateSocialMediaName(string name, string platformURL)
    {
        Result resultNameNull = Guard.AgainstNull(name, static () => SocialMediaNameException.NullSocialMediaName());
        if (!resultNameNull.IsSuccess)
        {
            return Result.Failure(ErrorType.Validation, resultNameNull.Errors);
        }

        Result resultPlatformURLNull = Guard.AgainstNull(name, static () => SocialMediaNameException.NullSocialMediaName());
        if (!resultPlatformURLNull.IsSuccess)
        {
            return Result.Failure(ErrorType.Validation, resultPlatformURLNull.Errors);
        }

        return Result.Combine(
            Guard.AgainstNullOrWhiteSpace(name, static () => SocialMediaNameException.EmptyName()),
            Guard.AgainstCondition(!ValidNamePattern.IsMatch(name), static () => SocialMediaNameException.InvalidCharacters()),
            Guard.AgainstRange(name.Length, 3, 50, static () => SocialMediaNameException.InvalidLength()),
            Guard.AgainstNullOrWhiteSpace(platformURL, static () => SocialMediaNameException.EmptyPlatformURL()),
            Guard.AgainstCondition(!Uri.IsWellFormedUriString(platformURL, UriKind.Absolute) || !platformURL.StartsWith("http", StringComparison.OrdinalIgnoreCase), static () => SocialMediaNameException.InvalidPlatformURLFormat())
        );
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return PlatformURL;
    }

    public bool IsEmpty() => this == Empty();

    public override string ToString() => $"{Name} ({PlatformURL})";
}
