using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Domain.Establishments;

public sealed class EstablishmentMember : BaseEntity<EstablishmentMemberId>
{
    // properties
    public EstablishmentMemberId EstablishmentMemberId => Id;

    public EstablishmentId EstablishmentId { get; private set; }
    public EmployeeId EmployeeId { get; private set; }
    public MemberTitle MemberTitle { get; private set; }
    public MemberDescription MemberDescription { get; private set; }
    public MemberTag MemberTag { get; private set; }

    // empty object
    private static readonly Lazy<EstablishmentMember> _empty = new(() => new EstablishmentMember());

    public static EstablishmentMember Empty() => _empty.Value;

    // navigation properties
    public Employee Employee { get; private set; } = default!;

    public Establishment Establishment { get; private set; } = default!;

    // constructor
    private EstablishmentMember() : base(EstablishmentMemberId.New())
        => (EstablishmentId, EmployeeId, MemberDescription, MemberTag, MemberTitle)
            = (EstablishmentId.Empty(), EmployeeId.Empty(), MemberDescription.Empty(), MemberTag.Empty(), MemberTitle.Empty());

    private EstablishmentMember(
        EstablishmentMemberId establishmentMemberId,
        EstablishmentId establishmentId,
        EmployeeId employeeId,
        MemberDescription description,
        MemberTag tag,
        MemberTitle title)
        : base(establishmentMemberId)
            => (EstablishmentId, EmployeeId, MemberDescription, MemberTag, MemberTitle)
                = (establishmentId, employeeId, description, tag, title);

    // factory method
    public static Result<EstablishmentMember> Create(
        EstablishmentId establishmentId,
        EmployeeId employeeId,
        MemberDescription description,
        MemberTag tag,
        MemberTitle title)
    {
        var result = Result.Combine(
            Guard.AgainstCondition(establishmentId == EstablishmentId.Empty(),
                static () => StronglyTypedIdException.NullId()),
            Guard.AgainstCondition(employeeId == EmployeeId.Empty(),
                static () => StronglyTypedIdException.NullId()),
            Guard.AgainstCondition(description.IsEmpty(),
                static () => MemberDescriptionException.EmptyDescription()),
            Guard.AgainstCondition(tag.IsEmpty(),
                static () => MemberTagException.EmptyTag()),
            Guard.AgainstCondition(title.IsEmpty(),
                static () => MemberTitleException.EmptyTitle())
        );

        return result.IsSuccess
            ? Result<EstablishmentMember>.Success(new EstablishmentMember(
                EstablishmentMemberId.New(),
                establishmentId,
                employeeId,
                description,
                tag,
                title))
            : Result<EstablishmentMember>.Failure(ErrorType.Validation, result.Errors);
    }

    // factory methods for updating properties
    public Result<EstablishmentMember> WithTitle(MemberTitle title)
    {
        Result result = Guard.AgainstCondition(title.IsEmpty(),
            static () => MemberTitleException.EmptyTitle());

        return result.IsSuccess
            ? Result<EstablishmentMember>.Success(new EstablishmentMember(
                EstablishmentMemberId,
                EstablishmentId,
                EmployeeId,
                MemberDescription,
                MemberTag,
                title))
            : Result<EstablishmentMember>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentMember> WithDescription(MemberDescription description)
    {
        Result result = Guard.AgainstCondition(description.IsEmpty(),
            static () => MemberDescriptionException.EmptyDescription());

        return result.IsSuccess
            ? Result<EstablishmentMember>.Success(new EstablishmentMember(
                EstablishmentMemberId,
                EstablishmentId,
                EmployeeId,
                description,
                MemberTag,
                MemberTitle))
            : Result<EstablishmentMember>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentMember> WithTag(MemberTag tag)
    {
        Result result = Guard.AgainstCondition(tag.IsEmpty(),
            static () => MemberTagException.EmptyTag());

        return result.IsSuccess
            ? Result<EstablishmentMember>.Success(new EstablishmentMember(
                EstablishmentMemberId,
                EstablishmentId,
                EmployeeId,
                MemberDescription,
                tag,
                MemberTitle))
            : Result<EstablishmentMember>.Failure(ErrorType.Validation, result.Errors);
    }
}
