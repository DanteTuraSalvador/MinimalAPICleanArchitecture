using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Domain.Employees;

public sealed class EmployeeRole : BaseEntity<EmployeeRoleId>
{
    public EmployeeRoleId EmployeeRoleId => Id;
    public RoleName RoleName { get; private set; }

    public bool IsEmpty() => this == Empty();

    private static readonly Lazy<EmployeeRole> _empty = new(() => new EmployeeRole());

    public static EmployeeRole Empty() => _empty.Value;

    private EmployeeRole() : base(EmployeeRoleId.Empty()) => RoleName = RoleName.Empty();

    private EmployeeRole(EmployeeRoleId employeeRoleId, RoleName roleName) : base(employeeRoleId) => RoleName = roleName;

    public static Result<EmployeeRole> Create(RoleName roleName)
    {
        var roleNameResult = RoleName.Create(roleName.Name);

        return roleNameResult.IsSuccess
            ? Result<EmployeeRole>.Success(new EmployeeRole(EmployeeRoleId.New(), roleNameResult.Value!))
            : Result<EmployeeRole>.Failure(ErrorType.Validation, roleNameResult.Errors);
    }

    public Result<EmployeeRole> WithRoleName(RoleName newRoleName)
    {
        var roleNameResult = RoleName.Create(newRoleName.Name);

        return roleNameResult.IsSuccess
            ? Result<EmployeeRole>.Success(new EmployeeRole(Id, roleNameResult.Value!))
            : Result<EmployeeRole>.Failure(ErrorType.Validation, roleNameResult.Errors);
    }

    public override string ToString() => RoleName.ToString();
}