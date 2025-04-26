using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record EmployeeId(Guid Value) : StronglyTypedId<EmployeeId>(Value)
{
    public EmployeeId() : this(Guid.NewGuid()) { }

    public static Result<EmployeeId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<EmployeeId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        return Result<EmployeeId>.Success(new EmployeeId(value));
    }

    public override string ToString() => Value.ToString();
}