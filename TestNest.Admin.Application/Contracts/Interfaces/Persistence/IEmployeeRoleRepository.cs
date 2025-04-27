using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Persistence;

// Interface defining the contract for an employee role repository.
//public interface IEmployeeRoleRepository
//{
//    Task<Result<EmployeeRole>> GetEmployeeRoleByNameAsync(string roleName);

//    Task<Result<EmployeeRole>> GetByIdAsync(EmployeeRoleId employeeRoleId);

//    Task<Result> AddAsync(EmployeeRole employeeRole);

//    Task<Result> UpdateAsync(EmployeeRole employeeRole);

//    Task<Result> DeleteAsync(EmployeeRoleId employeeRoleId);

//    Task DetachAsync(EmployeeRole employeeRole);

//    Task<Result<IEnumerable<EmployeeRole>>> ListAsync(ISpecification<EmployeeRole> spec);

//    Task<Result<int>> CountAsync(ISpecification<EmployeeRole> spec);

//    Task<bool> RoleIdExists(EmployeeRoleId roleId); // called by EmployeeService
//}

public interface IEmployeeRoleRepository : IGenericRepository<EmployeeRole, EmployeeRoleId>
{
    Task<Result<EmployeeRole>> GetEmployeeRoleByNameAsync(string roleName);

    Task DetachAsync(EmployeeRole employeeRole);

    Task<Result<IEnumerable<EmployeeRole>>> ListAsync(ISpecification<EmployeeRole> spec);

    Task<Result<int>> CountAsync(ISpecification<EmployeeRole> spec);

    Task<bool> RoleIdExists(EmployeeRoleId roleId);
}
