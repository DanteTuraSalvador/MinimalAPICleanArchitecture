using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record EstablishmentContactId(Guid Value) : StronglyTypedId<EstablishmentContactId>(Value)
{
    public EstablishmentContactId() : this(Guid.NewGuid()) { }

    public static Result<EstablishmentContactId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<EstablishmentContactId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        return Result<EstablishmentContactId>.Success(new EstablishmentContactId(value));
    }

    public override string ToString() => Value.ToString();
}