using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.Extensions;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Specifications.EmployeeRoleSpecifications;

public class EmployeeRoleSpecification : BaseSpecification<EmployeeRole>
{
    public EmployeeRoleSpecification(
        string employeeRoleId = null,
        string roleName = null,
        string sortBy = null,
        string sortDirection = "asc",
        int? pageNumber = null,
        int? pageSize = null
        )
    {
        var spec = new BaseSpecification<EmployeeRole>();

        if (!string.IsNullOrEmpty(employeeRoleId))
        {
            SharedLibrary.Common.Results.Result<EmployeeRoleId> employeeRoleIdValidatedResult = IdHelper.ValidateAndCreateId<EmployeeRoleId>(employeeRoleId);
            if (employeeRoleIdValidatedResult.IsSuccess)
            {
                var idSpec = new BaseSpecification<EmployeeRole>(e => e.Id == employeeRoleIdValidatedResult.Value!);
                spec = spec.And(idSpec);
            }
        }

        if (!string.IsNullOrEmpty(roleName))
        {
            var nameSpec = new BaseSpecification<EmployeeRole>(e => EF.Functions.Like(e.RoleName.Name.ToLower(), $"%{roleName.ToLower()}%"));
            spec = spec.And(nameSpec);
        }

        Criteria = spec.Criteria;

        if (!string.IsNullOrEmpty(sortBy))
        {
            SortDirection direction = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;
            switch (sortBy.ToLowerInvariant())
            {
                case "rolename":
                    AddOrderBy(e => e.RoleName.Name, direction);
                    break;

                case "id":
                case "employeeroleid":
                default:
                    AddOrderBy(e => e.Id, direction);
                    break;
            }
        }
        else
        {
            AddOrderBy(e => e.Id, SortDirection.Ascending);
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }

    public EmployeeRoleSpecification(EmployeeRoleId employeeRoleId) : base(e => e.Id == employeeRoleId)
    {
    }
}
