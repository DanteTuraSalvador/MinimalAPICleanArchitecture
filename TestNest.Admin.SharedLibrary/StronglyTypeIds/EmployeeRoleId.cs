using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds;

public sealed record EmployeeRoleId(Guid Value) : StronglyTypedId<EmployeeRoleId>(Value)
{
    public EmployeeRoleId() : this(Guid.NewGuid()) { }

    public static Result<EmployeeRoleId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            var exception = StronglyTypedIdException.NullId();
            return Result<EmployeeRoleId>.Failure(ErrorType.Validation,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        return Result<EmployeeRoleId>.Success(new EmployeeRoleId(value));
    }

    public override string ToString() => Value.ToString();
}