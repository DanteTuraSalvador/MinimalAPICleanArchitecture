using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Contracts.Interfaces.Persistence;

//public interface IEmployeeRepository
//{
//    Task<Result<Employee>> GetByIdAsync(EmployeeId employeeId);

//    Task<Result<Employee>> AddAsync(Employee employee);

//    Task<Result<Employee>> UpdateAsync(Employee employee);

//    Task<Result> DeleteAsync(EmployeeId employeeId);

//    Task DetachAsync(Employee employee);

//    Task<bool> EmployeeExistsWithSameCombination(
//        EmployeeId employeeId,
//        EmployeeNumber employeeNumber,
//        PersonName personName,
//        EmailAddress emailAddress,
//        EstablishmentId establishmentId);

//    Task<Result<IEnumerable<Employee>>> ListAsync(ISpecification<Employee> spec);

//    Task<Result<int>> CountAsync(ISpecification<Employee> spec);
//}

public interface IEmployeeRepository : IGenericRepository<Employee, EmployeeId>
{
    Task DetachAsync(Employee employee);

    Task<bool> EmployeeExistsWithSameCombination(
        EmployeeId employeeId,
        EmployeeNumber employeeNumber,
        PersonName personName,
        EmailAddress emailAddress,
        EstablishmentId establishmentId);

    Task<Result<IEnumerable<Employee>>> ListAsync(ISpecification<Employee> spec);

    Task<Result<int>> CountAsync(ISpecification<Employee> spec);
}
