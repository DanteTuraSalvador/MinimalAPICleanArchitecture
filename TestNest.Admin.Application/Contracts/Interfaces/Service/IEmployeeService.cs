using TestNest.Admin.Application.Specifications.EmployeeSpecifications;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

// Defines the contract for an Employee service.
public interface IEmployeeService
{
    Task<Result<Employee>> CreateEmployeeAsync(EmployeeForCreationRequest employeeForCreationRequest);

    Task<Result<Employee>> UpdateEmployeeAsync(EmployeeId employeeId, EmployeeForUpdateRequest employeeForUpdateRequest);

    Task<Result> DeleteEmployeeAsync(EmployeeId employeeId);

    Task<Result<Employee>> GetEmployeeByIdAsync(EmployeeId employeeId);

    //Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync();

    Task<Result<Employee>> PatchEmployeeAsync(EmployeeId employeeId, EmployeePatchRequest employeePatchRequest);

    Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync(EmployeeSpecification spec);

    Task<Result<int>> CountAsync(EmployeeSpecification spec);
}