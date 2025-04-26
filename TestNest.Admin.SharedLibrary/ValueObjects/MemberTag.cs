using System.Text.RegularExpressions;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class MemberTag : ValueObject
{
    private static readonly Regex ValidPattern = new("^[A-Za-z0-9 ]+$", RegexOptions.Compiled);
    private static readonly Lazy<MemberTag> _empty = new(() => new MemberTag());
    public string Tag { get; }

    public bool IsEmpty() => this == Empty();

    public static MemberTag Empty() => _empty.Value;

    private MemberTag() => Tag = string.Empty;

    private MemberTag(string value) => Tag = value;

    public static Result<MemberTag> Create(string tag)
    {
        Result validationResult = ValidateMemberTag(tag);
        return validationResult.IsSuccess
            ? Result<MemberTag>.Success(new MemberTag(tag))
            : Result<MemberTag>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public Result<MemberTag> WithTag(string newTag)
        => Create(newTag);

    private static Result ValidateMemberTag(string tag)
    {
        Result resultNull = Guard.AgainstNull(tag, static () => MemberTagException.NullMemberTag());
        if (!resultNull.IsSuccess)
        {
            return Result.Failure(ErrorType.Validation, resultNull.Errors);
        }

        return Result.Combine(
            Guard.AgainstNullOrWhiteSpace(tag, static () => MemberTagException.EmptyTag()),
            Guard.AgainstCondition(!ValidPattern.IsMatch(tag), static () => MemberTagException.InvalidFormat()),
            Guard.AgainstRange(tag.Length, 3, 100, static () => MemberTagException.LengthOutOfRange())
        );
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Tag;
    }

    public override string ToString() => Tag;
}
