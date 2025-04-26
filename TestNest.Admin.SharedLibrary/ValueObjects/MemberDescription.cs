using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.ValueObjects.Common;

namespace TestNest.Admin.SharedLibrary.ValueObjects;

public sealed class MemberDescription : ValueObject
{
    private static readonly Lazy<MemberDescription> _empty = new(() => new MemberDescription());
    public string Description { get; }

    public bool IsEmpty() => this == Empty();

    public static MemberDescription Empty() => _empty.Value;

    private MemberDescription() => Description = string.Empty;

    private MemberDescription(string value) => Description = value;

    public static Result<MemberDescription> Create(string description)
    {
        Result resultNull = Guard.AgainstNull(description, static () => MemberDescriptionException.NullEstablishmentMemberDescription());
        if (!resultNull.IsSuccess)
        {
            return Result<MemberDescription>.Failure(ErrorType.Validation, resultNull.Errors);
        }

        Result validationResult = ValidateMemberDescription(description);
        return validationResult.IsSuccess
            ? Result<MemberDescription>.Success(new MemberDescription(description))
            : Result<MemberDescription>.Failure(ErrorType.Validation, validationResult.Errors);
    }

    public static Result<MemberDescription> WithDescription(string newDescription)
    {
        Result resultNull = Guard.AgainstNull(newDescription, static () => MemberDescriptionException.NullEstablishmentMemberDescription());
        if (!resultNull.IsSuccess)
        {
            return Result<MemberDescription>.Failure(ErrorType.Validation, resultNull.Errors);
        }

        return Create(newDescription);
    }

    private static Result ValidateMemberDescription(string description) => Result.Combine(
            Guard.AgainstNullOrWhiteSpace(description, static () => MemberDescriptionException.EmptyDescription()),
            Guard.AgainstRange(description.Length, 5, 500, static () => MemberDescriptionException.LengthOutOfRange())
        );

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Description;
    }

    public override string ToString() => Description;
}
