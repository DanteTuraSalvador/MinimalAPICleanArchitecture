using System.Transactions;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.Application.Services.Base;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Services;

public class EmployeeRoleService(
    IEmployeeRoleRepository employeeRoleRepository,
    IUnitOfWork unitOfWork,
    IDatabaseExceptionHandlerFactory exceptionHandlerFactory,
    ILogger<EmployeeRoleService> logger) : BaseService(unitOfWork, logger, exceptionHandlerFactory), IEmployeeRoleService
{
    private readonly IEmployeeRoleRepository _employeeRoleRepository = employeeRoleRepository;

    public async Task<Result<EmployeeRole>> CreateEmployeeRoleAsync(
        EmployeeRoleForCreationRequest employeeRoleForCreationRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        Result<RoleName> roleNameResult = RoleName.Create(employeeRoleForCreationRequest.RoleName);

        if (!roleNameResult.IsSuccess)
        {
            return Result<EmployeeRole>.Failure(
                ErrorType.Validation,
                [.. roleNameResult.Errors]);
        }

        Result<EmployeeRole> existingRoleResult = await _employeeRoleRepository
            .GetEmployeeRoleByNameAsync(roleNameResult.Value!.Name);

        if (existingRoleResult.IsSuccess)
        {
            var exception = EmployeeRoleException.DuplicateResource();
            return Result<EmployeeRole>.Failure(
                ErrorType.Conflict,
                new Error(exception.Code.ToString(),
                    exception.Message.ToString()));
        }

        Result<EmployeeRole> employeeRoleResult = EmployeeRole.Create(roleNameResult.Value!);

        if (!employeeRoleResult.IsSuccess)
        {
            return Result<EmployeeRole>.Failure(
                ErrorType.Validation,
                [.. employeeRoleResult.Errors]);
        }

        EmployeeRole employeeRole = employeeRoleResult.Value!;
        _ = await _employeeRoleRepository.AddAsync(employeeRole);

        Result<EmployeeRole> commitResult = await SafeCommitAsync(
            () => Result<EmployeeRole>.Success(employeeRole));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return commitResult;
        }
        return commitResult;
    }

    public async Task<Result<EmployeeRole>> UpdateEmployeeRoleAsync(
        EmployeeRoleId employeeRoleId,
        EmployeeRoleForUpdateRequest employeeRoleForUpdateRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        Result<EmployeeRole> validatedEmployeeRole = await _employeeRoleRepository
            .GetByIdAsync(employeeRoleId);
        if (!validatedEmployeeRole.IsSuccess)
        {
            return validatedEmployeeRole;
        }

        EmployeeRole employeeRole = validatedEmployeeRole.Value!;
        await _employeeRoleRepository.DetachAsync(employeeRole);

        Result<RoleName> roleName = RoleName.Create(employeeRoleForUpdateRequest.RoleName);

        if (!roleName.IsSuccess)
        {
            return Result<EmployeeRole>.Failure(
                ErrorType.Validation,
                [.. roleName.Errors]);
        }

        Result<EmployeeRole> existingRoleResult = await _employeeRoleRepository.GetEmployeeRoleByNameAsync(roleName.Value!.Name);
        if (existingRoleResult.IsSuccess && existingRoleResult.Value!.Id != employeeRoleId)
        {
            var exception = EmployeeRoleException.DuplicateResource();
            return Result<EmployeeRole>.Failure(ErrorType.Conflict, new Error(exception.Code.ToString(), exception.Message.ToString()));
        }

        Result<EmployeeRole> updatedEmployeeRoleResult = employeeRole.WithRoleName(roleName.Value!);

        if (!updatedEmployeeRoleResult.IsSuccess)
        {
            return updatedEmployeeRoleResult;
        }

        Result<EmployeeRole> updateResult = await _employeeRoleRepository.UpdateAsync(updatedEmployeeRoleResult.Value!);
        if (!updateResult.IsSuccess)
        {
            return Result<EmployeeRole>.Failure(updateResult.ErrorType, updateResult.Errors);
        }

        Result<EmployeeRole> commitResult = await SafeCommitAsync(
            () => updateResult);

        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return commitResult;
        }
        return commitResult;
    }

    public async Task<Result> DeleteEmployeeRoleAsync(EmployeeRoleId employeeRoleId)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        Result result = await _employeeRoleRepository.DeleteAsync(employeeRoleId);
        if (!result.IsSuccess)
        {
            return result;
        }

        Result<bool> commitResult = await SafeCommitAsync<bool>(() => Result<bool>.Success(true));

        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result.Success();
        }
        return Result.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    public async Task<Result<EmployeeRole>> GetEmployeeRoleByIdAsync(EmployeeRoleId employeeRoleId)
        => await _employeeRoleRepository.GetByIdAsync(employeeRoleId);



    public async Task<Result<IEnumerable<EmployeeRole>>> GetEmployeeRolessAsync(ISpecification<EmployeeRole> spec)
        => await _employeeRoleRepository.ListAsync(spec);

  

    public async Task<Result<int>> CountAsync(ISpecification<EmployeeRole> spec)
        => await _employeeRoleRepository.CountAsync(spec);
}
