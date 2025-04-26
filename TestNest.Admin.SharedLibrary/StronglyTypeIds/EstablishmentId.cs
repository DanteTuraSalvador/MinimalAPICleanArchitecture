using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record EstablishmentId(Guid Value) : StronglyTypedId<EstablishmentId>(Value)
{
    public EstablishmentId() : this(Guid.NewGuid()) { }

    ////public static EstablishmentId Create(Guid value)
    ////    => value == Guid.Empty ? throw StronglyTypedIdException.NullId()
    ////        : new EstablishmentId(value);

    public static Result<EstablishmentId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<EstablishmentId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }
        return Result<EstablishmentId>.Success(new EstablishmentId(value));
    }
    public override string ToString() => Value.ToString();
}