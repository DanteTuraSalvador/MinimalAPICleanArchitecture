using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record EstablishmentPhoneId(Guid Value) : StronglyTypedId<EstablishmentPhoneId>(Value)
{
    public EstablishmentPhoneId() : this(Guid.NewGuid()) { }

    public static Result<EstablishmentPhoneId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<EstablishmentPhoneId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        return Result<EstablishmentPhoneId>.Success(new EstablishmentPhoneId(value));
    }

    public override string ToString() => Value.ToString();
}