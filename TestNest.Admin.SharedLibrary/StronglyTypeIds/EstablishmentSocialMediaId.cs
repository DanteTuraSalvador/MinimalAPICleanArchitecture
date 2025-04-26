using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record EstablishmentSocialMediaId(Guid Value) : StronglyTypedId<EstablishmentSocialMediaId>(Value)
{
    public EstablishmentSocialMediaId() : this(Guid.NewGuid()) { }

    public static Result<EstablishmentSocialMediaId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<EstablishmentSocialMediaId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        return Result<EstablishmentSocialMediaId>.Success(new EstablishmentSocialMediaId(value));
    }

    public override string ToString() => Value.ToString();
}