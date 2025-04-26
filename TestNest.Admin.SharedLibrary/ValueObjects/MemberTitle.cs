using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class MemberTitle : ValueObject
{
    private static readonly Regex ValidPattern = new("^[A-Za-z ]+$", RegexOptions.Compiled);
    private static readonly Lazy<MemberTitle> _empty = new(() => new MemberTitle());
    public string Title { get; }

    public bool IsEmpty() => this == Empty();

    public static MemberTitle Empty() => _empty.Value;

    private MemberTitle() => Title = string.Empty;

    private MemberTitle(string value) => Title = value;

    public static Result<MemberTitle> Create(string memberTitle)
    {
        Result validationResult = ValidateMemberTitle(memberTitle);
        return validationResult.IsSuccess
            ? Result<MemberTitle>.Success(new MemberTitle(memberTitle))
            : Result<MemberTitle>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<MemberTitle> WithTitle(string newMemberTitle)
        => Create(newMemberTitle);

    private static Result ValidateMemberTitle(string memberTitle)
    {
        Result resultNull = Guard.AgainstNull(memberTitle, static () => MemberTitleException.NullMemberTitle());
        if (!resultNull.IsSuccess)
        {
            return Result.Failure(ErrorType.Validation, resultNull.Errors);
        }

        return Result.Combine(
            Guard.AgainstNullOrWhiteSpace(memberTitle, static () => MemberTitleException.EmptyTitle()),
            Guard.AgainstCondition(!ValidPattern.IsMatch(memberTitle), static () => MemberTitleException.InvalidFormat()),
            Guard.AgainstRange(memberTitle.Length, 2, 100, static () => MemberTitleException.LengthOutOfRange())
        );
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Title;
    }

    public override string ToString() => Title;
}
