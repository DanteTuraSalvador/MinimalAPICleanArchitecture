using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record EstablishmentMemberId(Guid Value) : StronglyTypedId<EstablishmentMemberId>(Value)
{
    public EstablishmentMemberId() : this(Guid.NewGuid()) { }

    public static Result<EstablishmentMemberId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<EstablishmentMemberId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        return Result<EstablishmentMemberId>.Success(new EstablishmentMemberId(value));
    }

    public override string ToString() => Value.ToString();
}