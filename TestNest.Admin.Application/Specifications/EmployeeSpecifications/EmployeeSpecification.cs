using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.Extensions;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.Application.Specifications.EmployeeSpecifications;

public class EmployeeSpecification : BaseSpecification<Employee>
{
    public EmployeeSpecification(
        string employeeNumber = null,
        string firstName = null,
        string middleName = null,
        string lastName = null,
        string emailAddress = null,
        int? employeeStatusId = null,
        string employeeRoleId = null,
        string establishmentId = null,
        string sortBy = "EmployeeId",
        string sortDirection = "asc",
        int? pageNumber = 1,
        int? pageSize = 10)
    {
        var spec = new BaseSpecification<Employee>();

        if (!string.IsNullOrEmpty(employeeNumber))
        {
            var numberSpec = new BaseSpecification<Employee>(
                e => EF.Functions.Like(e.EmployeeNumber.EmployeeNo.ToLowerInvariant(), $"%{employeeNumber.ToLowerInvariant()}%"));
            spec = spec.And(numberSpec);
        }

        if (!string.IsNullOrEmpty(firstName))
        {
            var firstNameSpec = new BaseSpecification<Employee>(
                e => EF.Functions.Like(e.EmployeeName.FirstName.ToLowerInvariant(), $"%{firstName.ToLowerInvariant()}%"));
            spec = spec.And(firstNameSpec);
        }

        if (!string.IsNullOrEmpty(middleName))
        {
            var middleNameSpec = new BaseSpecification<Employee>(
                e => EF.Functions.Like(e.EmployeeName.MiddleName.ToLowerInvariant(), $"%{middleName.ToLowerInvariant()}%"));
            spec = spec.And(middleNameSpec);
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            var lastNameSpec = new BaseSpecification<Employee>(
                e => EF.Functions.Like(e.EmployeeName.LastName.ToLowerInvariant(), $"%{lastName.ToLower()}%"));
            spec = spec.And(lastNameSpec);
        }

        if (!string.IsNullOrEmpty(emailAddress))
        {
            var emailAddressSpec = new BaseSpecification<Employee>(
                e => EF.Functions.Like(e.EmployeeEmail.Email.ToLowerInvariant(), $"%{emailAddress.ToLowerInvariant()}%"));
            spec = spec.And(emailAddressSpec);
        }

        if (employeeStatusId.HasValue)
        {
            Result<EmployeeStatus> statusResult = EmployeeStatus.FromId(employeeStatusId.Value);

            if (statusResult.IsSuccess)
            {
                EmployeeStatus? statusToFilter = statusResult.Value;
                var statusIdSpec = new BaseSpecification<Employee>(e => e.EmployeeStatus == statusToFilter!);
                spec = spec.And(statusIdSpec);
            }
        }

        if (!string.IsNullOrEmpty(employeeRoleId))
        {
            Result<EmployeeRoleId> employeeRoleIdValidatedResult = IdHelper.ValidateAndCreateId<EmployeeRoleId>(employeeRoleId);
            if (employeeRoleIdValidatedResult.IsSuccess)
            {
                var roleIdSpec = new BaseSpecification<Employee>(e => e.EmployeeRoleId == employeeRoleId);
                spec = spec.And(roleIdSpec);
            }
        }

        if (!string.IsNullOrEmpty(establishmentId))
        {
            Result<EstablishmentId> establishmentIdValidatedResult = IdHelper.ValidateAndCreateId<EstablishmentId>(establishmentId);
            if (establishmentIdValidatedResult.IsSuccess)
            {
                var estIdSpec = new BaseSpecification<Employee>(e => e.EstablishmentId == establishmentId);
                spec = spec.And(estIdSpec);
            }
        }

        Criteria = spec.Criteria;

        if (!string.IsNullOrEmpty(sortBy))
        {
            SortDirection direction = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;
            switch (sortBy.ToLowerInvariant())
            {
                case "employeenumber":
                    AddOrderBy(e => e.EmployeeNumber.EmployeeNo, direction);
                    break;

                case "firstname":
                    AddOrderBy(e => e.EmployeeName.FirstName, direction);
                    break;

                case "middlename":
                    AddOrderBy(e => e.EmployeeName.MiddleName!, direction);
                    break;

                case "lastname":
                    AddOrderBy(e => e.EmployeeName.LastName, direction);
                    break;

                case "emailaddress":
                    AddOrderBy(e => e.EmployeeEmail.Email, direction);
                    break;

                case "employeestatusid":
                    AddOrderBy(e => EF.Property<int>(e, "EmployeeStatusId"), direction);
                    break;

                case "employeeroleid":
                    AddOrderBy(e => e.EmployeeRoleId, direction);
                    break;

                case "establishmentid":
                    AddOrderBy(e => e.EstablishmentId, direction);
                    break;

                case "employeeid":
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

    public EmployeeSpecification(EmployeeId employeeId) : base(e => e.Id == employeeId)
    {
    }

    public EmployeeSpecification(EmployeeNumber employeeNumber) : base(e => e.EmployeeNumber == employeeNumber)
    {
    }
}
