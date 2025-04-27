using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.Infrastructure.Persistence.Common;
using TestNest.Admin.Infrastructure.Persistence.Repositories.Common;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Infrastructure.Persistence.Repositories;

public class EmployeeRepository(ApplicationDbContext employeeDbContext) : GenericRepository<Employee, EmployeeId>(employeeDbContext), IEmployeeRepository
{
    private readonly ApplicationDbContext _employeeDbContext = employeeDbContext;

    public async Task<Result<IEnumerable<Employee>>> ListAsync(ISpecification<Employee> spec)
    {
        try
        {
            IQueryable<Employee> query = _employeeDbContext.Employees
                .Include(x => x.EmployeeName)
                .Include(x => x.EmployeeEmail)
                .Include(x => x.EmployeeRole)
                .Include(x => x.Establishment)
                .AsQueryable();

            var employeeSpc = (BaseSpecification<Employee>)spec;
            query = SpecificationEvaluator<Employee>.GetQuery(query, employeeSpc);

            List<Employee> employees = await query.ToListAsync();
            return Result<IEnumerable<Employee>>.Success(employees);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Employee>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<int>> CountAsync(ISpecification<Employee> spec)
    {
        try
        {
            IQueryable<Employee> query = SpecificationEvaluator<Employee>.GetQuery(
                _employeeDbContext.Employees,
                (BaseSpecification<Employee>)spec);
            int count = await query.CountAsync();
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<Employee>> GetByIdAsync(EmployeeId employeeId)
    {
        try
        {
            Employee? employee = await _employeeDbContext.Employees
                .Include(x => x.EmployeeName)
                .Include(x => x.EmployeeEmail)
                .Include(x => x.EmployeeRole)
                .Include(x => x.Establishment)
                .FirstOrDefaultAsync(x => x.Id == employeeId);

            if (employee == null)
            {
                var exception = EmployeeException.NotFound();
                return Result<Employee>.Failure(
                    ErrorType.NotFound,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }
            return Result<Employee>.Success(employee);
        }
        catch (Exception ex)
        {
            return Result<Employee>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<Employee>> AddAsync(Employee employee)
    {
        try
        {
            _ = await _employeeDbContext.Employees.AddAsync(employee);
            return Result<Employee>.Success(employee);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            return Result<Employee>.Failure(ErrorType.Conflict,
                new Error("DuplicateEmployeeNumber", "Employee number already exists"));
        }
        catch (Exception ex)
        {
            return Result<Employee>.Failure(ErrorType.Internal,
                new Error("AddFailed", ex.Message));
        }
    }

    public override async Task<Result<Employee>> UpdateAsync(Employee employee)
    {
        try
        {
            bool exists = await _employeeDbContext.Employees
                .AsNoTracking()
                .AnyAsync(x => x.Id == employee.Id);

            if (!exists)
            {
                var exception = EmployeeException.NotFound();
                return Result<Employee>.Failure(ErrorType.NotFound,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }
            _ = _employeeDbContext.Update(employee);
            return Result<Employee>.Success(employee);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            return Result<Employee>.Failure(ErrorType.Conflict,
                new Error("DuplicateEmployeeNumber", "An employee with that number already exists."));
        }
        catch (Exception ex)
        {
            return Result<Employee>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result> DeleteAsync(EmployeeId employeeId)
    {
        try
        {
            int rowsDeleted = await _employeeDbContext.Employees
                .Where(p => p.Id == employeeId)
                .ExecuteDeleteAsync();

            return rowsDeleted > 0
                ? Result.Success()
                : Result.Failure(ErrorType.NotFound,
                    new Error(EmployeeException.NotFound().Code.ToString(),
                        EmployeeException.NotFound().Message.ToString()));
        }
        catch (Exception ex)
        {
            return Result.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public async Task DetachAsync(Employee employee)
    {
        _employeeDbContext.Entry(employee).State = EntityState.Detached;
        await Task.CompletedTask;
    }

    public async Task<bool> EmployeeExistsWithSameCombination(
        EmployeeId employeeId,
        EmployeeNumber employeeNumber,
        PersonName personName,
        EmailAddress emailAddress,
        EstablishmentId establishmentId)
            => await _employeeDbContext.Employees.AnyAsync(
                e => e.EmployeeNumber.EmployeeNo == employeeNumber.EmployeeNo &&
                     e.EmployeeName.FirstName == personName.FirstName &&
                     e.EmployeeName.LastName == personName.LastName &&
                     e.EmployeeEmail.Email == emailAddress.Email &&
                     e.EstablishmentId == establishmentId &&
                     e.Id != employeeId);
}

//public class EmployeeRepository(ApplicationDbContext employeeDbContext) : IEmployeeRepository
//{
//    private readonly ApplicationDbContext _employeeDbContext = employeeDbContext;

//    public async Task<Result<IEnumerable<Employee>>> ListAsync(ISpecification<Employee> spec)
//    {
//        try
//        {
//            IQueryable<Employee> query = _employeeDbContext.Employees
//                .Include(x => x.EmployeeName)
//                .Include(x => x.EmployeeEmail)
//                .Include(x => x.EmployeeRole)
//                .Include(x => x.Establishment)
//                .AsQueryable();

//            var employeeSpc = (BaseSpecification<Employee>)spec;
//            query = SpecificationEvaluator<Employee>.GetQuery(query, employeeSpc);

//            List<Employee> employees = await query.ToListAsync();
//            return Result<IEnumerable<Employee>>.Success(employees);
//        }
//        catch (Exception ex)
//        {
//            return Result<IEnumerable<Employee>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<int>> CountAsync(ISpecification<Employee> spec)
//    {
//        try
//        {
//            IQueryable<Employee> query = SpecificationEvaluator<Employee>.GetQuery(
//                _employeeDbContext.Employees,
//                (BaseSpecification<Employee>)spec);
//            int count = await query.CountAsync();
//            return Result<int>.Success(count);
//        }
//        catch (Exception ex)
//        {
//            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<Employee>> GetByIdAsync(EmployeeId employeeId)
//    {
//        try
//        {
//            Employee? employee = await _employeeDbContext.Employees
//                .Include(x => x.EmployeeName)
//                .Include(x => x.EmployeeEmail)
//                .Include(x => x.EmployeeRole)
//                .Include(x => x.Establishment)
//                .FirstOrDefaultAsync(x => x.Id == employeeId);

//            if (employee == null)
//            {
//                var exception = EmployeeException.NotFound();
//                return Result<Employee>.Failure(
//                    ErrorType.NotFound,
//                    new Error(exception.Code.ToString(), exception.Message.ToString()));
//            }
//            return Result<Employee>.Success(employee);
//        }
//        catch (Exception ex)
//        {
//            return Result<Employee>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<Employee>> AddAsync(Employee employee)
//    {
//        try
//        {
//            _ = await _employeeDbContext.Employees.AddAsync(employee);
//            return Result<Employee>.Success(employee);
//        }
//        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
//        {
//            return Result<Employee>.Failure(ErrorType.Conflict,
//                new Error("DuplicateEmployeeNumber", "Employee number already exists"));
//        }
//        catch (Exception ex)
//        {
//            return Result<Employee>.Failure(ErrorType.Internal,
//                new Error("AddFailed", ex.Message));
//        }
//    }

//    public async Task<Result<Employee>> UpdateAsync(Employee employee)
//    {
//        try
//        {
//            bool exists = await _employeeDbContext.Employees
//                .AsNoTracking()
//                .AnyAsync(x => x.Id == employee.Id);

//            if (!exists)
//            {
//                var exception = EmployeeException.NotFound();
//                return Result<Employee>.Failure(ErrorType.NotFound,
//                    new Error(exception.Code.ToString(), exception.Message.ToString()));
//            }
//            _ = _employeeDbContext.Update(employee);
//            return Result<Employee>.Success(employee);
//        }
//        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
//        {
//            return Result<Employee>.Failure(ErrorType.Conflict,
//                new Error("DuplicateEmployeeNumber", "An employee with that number already exists."));
//        }
//        catch (Exception ex)
//        {
//            return Result<Employee>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> DeleteAsync(EmployeeId employeeId)
//    {
//        try
//        {
//            int rowsDeleted = await _employeeDbContext.Employees
//                .Where(p => p.Id == employeeId)
//                .ExecuteDeleteAsync();

//            return rowsDeleted > 0
//                ? Result.Success()
//                : Result.Failure(ErrorType.NotFound,
//                    new Error(EmployeeException.NotFound().Code.ToString(),
//                        EmployeeException.NotFound().Message.ToString()));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task DetachAsync(Employee employee)
//    {
//        _employeeDbContext.Entry(employee).State = EntityState.Detached;
//        await Task.CompletedTask;
//    }

//    public async Task<bool> EmployeeExistsWithSameCombination(
//        EmployeeId employeeId,
//        EmployeeNumber employeeNumber,
//        PersonName personName,
//        EmailAddress emailAddress,
//        EstablishmentId establishmentId)
//            => await _employeeDbContext.Employees.AnyAsync(
//                e => e.EmployeeNumber.EmployeeNo == employeeNumber.EmployeeNo &&
//                    e.EmployeeName.FirstName == personName.FirstName &&
//                    e.EmployeeName.LastName == personName.LastName &&
//                    e.EmployeeEmail.Email == emailAddress.Email &&
//                    e.EstablishmentId == establishmentId &&
//                    e.Id != employeeId);
//}
