using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Dtos.Responses;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEmployeeRoleService
{
    Task<Result<EmployeeRoleResponse>> CreateEmployeeRoleAsync(
        EmployeeRoleForCreationRequest employeeRoleForCreationRequest);

    Task<Result<EmployeeRoleResponse>> UpdateEmployeeRoleAsync(
        EmployeeRoleId employeeRoleId,
        EmployeeRoleForUpdateRequest employeeRoleForUpdateRequest);

    Task<Result> DeleteEmployeeRoleAsync(EmployeeRoleId employeeRoleId);

    Task<Result<EmployeeRoleResponse>> GetEmployeeRoleByIdAsync(EmployeeRoleId employeeRoleId);

    Task<Result<IEnumerable<EmployeeRoleResponse>>> GetEmployeeRolessAsync(ISpecification<EmployeeRole> spec);

    Task<Result<int>> CountAsync(ISpecification<EmployeeRole> spec);
}
