using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record EstablishmentAddressId(Guid Value) : StronglyTypedId<EstablishmentAddressId>(Value)
{
    public EstablishmentAddressId() : this(Guid.NewGuid()) { }

    public static Result<EstablishmentAddressId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<EstablishmentAddressId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        return Result<EstablishmentAddressId>.Success(new EstablishmentAddressId(value));
    }

    public override string ToString() => Value.ToString();
}