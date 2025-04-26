using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record SocialMediaId(Guid Value) : StronglyTypedId<SocialMediaId>(Value)
{
    public SocialMediaId() : this(Guid.NewGuid()) { }

    public static Result<SocialMediaId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<SocialMediaId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        return Result<SocialMediaId>.Success(new SocialMediaId(value));
    }

    public override string ToString() => Value.ToString();
}