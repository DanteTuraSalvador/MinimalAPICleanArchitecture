using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class SocialMediaAccountName : ValueObject
{
    private static readonly Regex pattern = new("^[A-Za-z0-9_.]+$", RegexOptions.Compiled);
    private static readonly Lazy<SocialMediaAccountName> _empty = new(() => new SocialMediaAccountName());
    public string AccountName { get; }

    public bool IsEmpty() => this == Empty();

    public static SocialMediaAccountName Empty() => _empty.Value;

    public SocialMediaAccountName() => AccountName = string.Empty;

    private SocialMediaAccountName(string value) => AccountName = value;

    public static Result<SocialMediaAccountName> Create(string accountName)
    {
        var validationResult = ValidateSocialMediaAccountName(accountName);
        return validationResult.IsSuccess
            ? Result<SocialMediaAccountName>.Success(new SocialMediaAccountName(accountName!))
            : Result<SocialMediaAccountName>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<SocialMediaAccountName> Update(string newName)
        => Create(newName);

    private static Result ValidateSocialMediaAccountName(string accountName)
    {
        var resultNull = Guard.AgainstNull(accountName, () => SocialMediaAccountNameException.NullSocialMediaAccountName());
        if (!resultNull.IsSuccess)
            return Result.Failure(ErrorType.Validation, resultNull.Errors);

        return Result.Combine(
            Guard.AgainstNullOrWhiteSpace(accountName, () => SocialMediaAccountNameException.EmptyAccountName()),
            Guard.AgainstRange(accountName.Length, 3, 50, () => SocialMediaAccountNameException.InvalidLength()),
            Guard.AgainstCondition(accountName != null && !pattern.IsMatch(accountName), () => SocialMediaAccountNameException.InvalidCharacters())
        );
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return AccountName;
    }

    public override string ToString() => AccountName;
}