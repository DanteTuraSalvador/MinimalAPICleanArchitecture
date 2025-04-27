using System.Transactions;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Contracts.Interfaces.Service;
using TestNest.Admin.Application.Interfaces;
using TestNest.Admin.Application.Mappings;
using TestNest.Admin.Application.Services.Base;
using TestNest.Admin.Application.Specifications.EmployeeSpecifications;
using TestNest.Admin.Domain.Employees;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Employee;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.Helpers;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.Application.Services;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    IEmployeeRoleRepository employeeRoleRepository,
    IEstablishmentRepository establishmentRepository,
    IUnitOfWork unitOfWork,
    IDatabaseExceptionHandlerFactory exceptionHandlerFactory,
    ILogger<EmployeeService> logger) : BaseService(unitOfWork, logger, exceptionHandlerFactory), IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly IEmployeeRoleRepository _employeeRoleRepository = employeeRoleRepository;
    private readonly IEstablishmentRepository _establishmentRepository = establishmentRepository;

    public async Task<Result<EmployeeResponse>> CreateEmployeeAsync(
        EmployeeForCreationRequest employeeForCreationRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<EmployeeNumber> employeeNumberResult = EmployeeNumber.Create(employeeForCreationRequest.EmployeeNumber);
        Result<PersonName> employeeNameResult = PersonName.Create(employeeForCreationRequest.FirstName,
            employeeForCreationRequest.MiddleName, employeeForCreationRequest.LastName);
        Result<EmailAddress> employeeEmailResult = EmailAddress.Create(employeeForCreationRequest.EmailAddress);
        Result<EmployeeRoleId> employeeRoleIdResult = EmployeeRoleId.Create(employeeForCreationRequest.EmployeeRoleId);
        Result<EstablishmentId> establishmentIdResult = EstablishmentId.Create(employeeForCreationRequest.EstablishmentId);

        var combinedValidationResult = Result.Combine(
            employeeNumberResult.ToResult(),
            employeeNameResult.ToResult(),
            employeeEmailResult.ToResult(),
            employeeRoleIdResult.ToResult(),
            establishmentIdResult.ToResult());

        if (!combinedValidationResult.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                ErrorType.Validation,
                [.. combinedValidationResult.Errors]);
        }

        Result<bool> uniquenessCheckResult = await EmployeeCombinationExistsAsync(
            employeeNumberResult.Value!,
            employeeNameResult.Value!,
            employeeEmailResult.Value!,
            establishmentIdResult.Value!);

        if (!uniquenessCheckResult.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                uniquenessCheckResult.ErrorType,
                uniquenessCheckResult.Errors);
        }

        Result<Employee> employeeResult = Employee
            .Create(employeeNumberResult.Value!,
                employeeNameResult.Value!,
                employeeEmailResult.Value!,
                employeeRoleIdResult.Value!,
                establishmentIdResult.Value!);

        if (!employeeResult.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                ErrorType.Validation,
                [.. employeeResult.Errors]);
        }

        Employee employee = employeeResult.Value!;
        _ = await _employeeRepository.AddAsync(employee);

        Result<Employee> commitResult = await SafeCommitAsync(() => Result<Employee>.Success(employee));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EmployeeResponse>.Success(employee.ToEmployeeResponse());
        }
        return Result<EmployeeResponse>.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    public async Task<Result<EmployeeResponse>> UpdateEmployeeAsync(
        EmployeeId employeeId,
        EmployeeForUpdateRequest employeeForUpdateRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<Employee> validatedEmployee = await _employeeRepository.GetByIdAsync(employeeId);
        if (!validatedEmployee.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                validatedEmployee.ErrorType,
                validatedEmployee.Errors);
        }

        Employee employee = validatedEmployee.Value!;
        await _employeeRepository.DetachAsync(employee);

        Result<EmployeeNumber> employeeNumber = EmployeeNumber.Create(employeeForUpdateRequest.EmployeeNumber);
        Result<PersonName> employeeName = PersonName.Create(employeeForUpdateRequest.FirstName,
            employeeForUpdateRequest.MiddleName, employeeForUpdateRequest.LastName);
        Result<EmailAddress> employeeEmail = EmailAddress.Create(employeeForUpdateRequest.EmailAddress);
        Result<EmployeeRoleId> employeeRoleId = EmployeeRoleId.Create(employeeForUpdateRequest.EmployeeRoleId);
        Result<EstablishmentId> establishmentId = EstablishmentId.Create(employeeForUpdateRequest.EstablishmentId);
        Result<EmployeeStatus> employeeStatusResult = EmployeeStatus.FromId(employeeForUpdateRequest.EmployeeStatusId);

        if (!employeeStatusResult.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                employeeStatusResult.ErrorType,
                employeeStatusResult.Errors
            );
        }

        var combinedValidationResult = Result.Combine(
            employeeNumber.ToResult(),
            employeeName.ToResult(),
            employeeEmail.ToResult(),
            employeeRoleId.ToResult(),
            establishmentId.ToResult(),
            employeeStatusResult.ToResult()
        );

        if (!combinedValidationResult.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                ErrorType.Validation,
                [.. combinedValidationResult.Errors]);
        }

        Employee updatedEmployeeResult = employee
            .WithEmployeeNumber(employeeNumber.Value!)
            .Bind(e => e.WithPersonName(employeeName.Value!))
            .Bind(e => e.WithEmail(employeeEmail.Value!))
            .Bind(e => e.WithEmployeeRole(employeeRoleId.Value!))
            .Bind(e => e.WithEstablishmentId(establishmentId.Value!))
            .Bind(e => e.WithEmployeeStatus(employeeStatusResult.Value!)).Value!;

        Result<bool> uniquenessCheckResult = await EmployeeCombinationExistsAsync(
            employeeNumber.Value!,
            employeeName.Value!,
            employeeEmail.Value!,
            establishmentId.Value!,
            employeeId);

        if (!uniquenessCheckResult.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                uniquenessCheckResult.ErrorType,
                uniquenessCheckResult.Errors);
        }

        Result<Employee> updateResult = await _employeeRepository.UpdateAsync(updatedEmployeeResult);
        if (!updateResult.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                updateResult.ErrorType,
                updateResult.Errors);
        }

        Result<Employee> commitResult = await SafeCommitAsync(() => Result<Employee>.Success(updatedEmployeeResult));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EmployeeResponse>.Success(updatedEmployeeResult.ToEmployeeResponse());
        }
        return Result<EmployeeResponse>.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    public async Task<Result<EmployeeResponse>> PatchEmployeeAsync(
        EmployeeId employeeId,
        EmployeePatchRequest employeePatchRequest)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result<Employee> validatedEmployee = await _employeeRepository.GetByIdAsync(employeeId);
        if (!validatedEmployee.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                validatedEmployee.ErrorType,
                validatedEmployee.Errors);
        }

        Employee employee = validatedEmployee.Value!;
        await _employeeRepository.DetachAsync(employee);

        if (employeePatchRequest.EmployeeNumber != null)
        {
            Result<EmployeeNumber> employeeNumberResult = EmployeeNumber.Create(employeePatchRequest.EmployeeNumber);
            if (!employeeNumberResult.IsSuccess)
            {
                return Result<EmployeeResponse>.Failure(
                    employeeNumberResult.ErrorType,
                    employeeNumberResult.Errors);
            }
            employee = employee.WithEmployeeNumber(employeeNumberResult.Value!).Value!;
        }

        if (employeePatchRequest.FirstName != null || employeePatchRequest.MiddleName != null || employeePatchRequest.LastName != null)
        {
            Result<PersonName> nameResult = PersonName.Create(
                employeePatchRequest.FirstName ?? employee.EmployeeName.FirstName,
                employeePatchRequest.MiddleName ?? employee.EmployeeName.MiddleName,
                employeePatchRequest.LastName ?? employee.EmployeeName.LastName);

            if (!nameResult.IsSuccess)
            {
                return Result<EmployeeResponse>.Failure(
                    nameResult.ErrorType,
                    nameResult.Errors);
            }
            employee = employee.WithPersonName(nameResult.Value!).Value!;
        }

        if (employeePatchRequest.EmailAddress != null)
        {
            Result<EmailAddress> emailResult = EmailAddress.Create(employeePatchRequest.EmailAddress);
            if (!emailResult.IsSuccess)
            {
                return Result<EmployeeResponse>.Failure(
                    emailResult.ErrorType,
                    emailResult.Errors);
            }
            employee = employee.WithEmail(emailResult.Value!).Value!;
        }

        if (employeePatchRequest.EmployeeRoleId != null)
        {
            Result<EmployeeRoleId> roleIdResult = IdHelper.ValidateAndCreateId<EmployeeRoleId>(
                employeePatchRequest.EmployeeRoleId);

            if (!roleIdResult.IsSuccess)
            {
                return Result<EmployeeResponse>.Failure(
                    roleIdResult.ErrorType,
                    roleIdResult.Errors);
            }

            bool roleExists = await _employeeRoleRepository.RoleIdExists(roleIdResult.Value!);
            if (!roleExists)
            {
                return Result<EmployeeResponse>.Failure(
                    ErrorType.NotFound,
                    new Error(EmployeeException.InvalidEmployeeRoleId.Code.ToString(),
                        EmployeeException.InvalidEmployeeRoleId.Message));
            }
            employee = employee.WithEmployeeRole(roleIdResult.Value!).Value!;
        }

        if (employeePatchRequest.EstablishmentId != null)
        {
            Result<EstablishmentId> establishmentIdResult = IdHelper
                .ValidateAndCreateId<EstablishmentId>(employeePatchRequest.EstablishmentId);

            if (!establishmentIdResult.IsSuccess)
            {
                return Result<EmployeeResponse>.Failure(
                    establishmentIdResult.ErrorType,
                    establishmentIdResult.Errors);
            }

            bool establishmentExists = await _establishmentRepository
                .EstablishmentIdExists(establishmentIdResult.Value!);

            if (!establishmentExists)
            {
                return Result<EmployeeResponse>.Failure(
                    ErrorType.NotFound,
                    new Error(EmployeeException.InvalidEstablishmentId.Code.ToString(),
                        EmployeeException.InvalidEstablishmentId.Message));
            }
            employee = employee.WithEstablishmentId(establishmentIdResult.Value!).Value!;
        }

        if (employeePatchRequest.EmployeeStatusId != null)
        {
            Result<EmployeeStatus> statusResult = EmployeeStatus.FromId(employeePatchRequest.EmployeeStatusId.Value);
            if (!statusResult.IsSuccess)
            {
                return Result<EmployeeResponse>.Failure(statusResult.ErrorType, statusResult.Errors);
            }
            employee = employee.WithEmployeeStatus(statusResult.Value!).Value!;
        }

        if (employeePatchRequest.EmployeeNumber != null ||
            employeePatchRequest.FirstName != null ||
            employeePatchRequest.MiddleName != null ||
            employeePatchRequest.LastName != null ||
            employeePatchRequest.EstablishmentId != null ||
            employeePatchRequest.EmailAddress != null)
        {
            EmployeeNumber employeeNumberToCheck = employeePatchRequest.EmployeeNumber != null
                ? EmployeeNumber.Create(employeePatchRequest.EmployeeNumber).Value!
                : employee.EmployeeNumber;

            Result<PersonName> personNameToCheckResult = PersonName.Create(
                employeePatchRequest.FirstName ?? employee.EmployeeName.FirstName,
                employeePatchRequest.MiddleName ?? employee.EmployeeName.MiddleName,
                employeePatchRequest.LastName ?? employee.EmployeeName.LastName
            );

            if (!personNameToCheckResult.IsSuccess)
            {
                return Result<EmployeeResponse>.Failure(
                    personNameToCheckResult.ErrorType,
                    personNameToCheckResult.Errors);
            }
            PersonName personNameToCheck = personNameToCheckResult.Value!;

            EmailAddress emailToCheck = employeePatchRequest.EmailAddress != null
                ? EmailAddress.Create(employeePatchRequest.EmailAddress).Value!
                : employee.EmployeeEmail;

            EstablishmentId establishmentIdToCheck = employeePatchRequest.EstablishmentId != null
                ? IdHelper.ValidateAndCreateId<EstablishmentId>(employeePatchRequest.EstablishmentId).Value!
                : employee.EstablishmentId;

            Result<bool> uniquenessCheckResult = await EmployeeCombinationExistsAsync(
                employeeNumberToCheck,
                personNameToCheck,
                emailToCheck,
                establishmentIdToCheck,
                employeeId);

            if (!uniquenessCheckResult.IsSuccess)
            {
                return Result<EmployeeResponse>.Failure(
                    ErrorType.Conflict,
                    new Error(EmployeeException.DuplicateResource.Code.ToString(),
                        EmployeeException.DuplicateEmployeeErrorMessage));
            }
        }

        Result<Employee> updateResult = await _employeeRepository.UpdateAsync(employee);
        if (!updateResult.IsSuccess)
        {
            return Result<EmployeeResponse>.Failure(
                updateResult.ErrorType,
                updateResult.Errors);
        }

        Result<Employee> commitResult = await SafeCommitAsync(() => Result<Employee>.Success(employee));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result<EmployeeResponse>.Success(employee.ToEmployeeResponse());
        }
        return Result<EmployeeResponse>.Failure(
            commitResult.ErrorType,
            commitResult.Errors);
    }

    public async Task<Result> DeleteEmployeeAsync(EmployeeId employeeId)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Result result = await _employeeRepository.DeleteAsync(employeeId);
        if (!result.IsSuccess)
        {
            return result;
        }

        Result<bool> commitResult = await SafeCommitAsync(() => Result<bool>.Success(true));
        if (commitResult.IsSuccess)
        {
            scope.Complete();
            return Result.Success();
        }
        return Result.Failure(commitResult.ErrorType, commitResult.Errors);
    }

    private async Task<Result<bool>> EmployeeCombinationExistsAsync(
        EmployeeNumber employeeNumber,
        PersonName personName,
        EmailAddress emailAddress,
        EstablishmentId establishmentId,
        EmployeeId? employeeId = null)
    {
        Result<EmployeeId> idResult = employeeId == null
            ? IdHelper.ValidateAndCreateId<EmployeeId>(Guid.NewGuid().ToString())
            : IdHelper.ValidateAndCreateId<EmployeeId>(employeeId.Value.ToString());

        if (!idResult.IsSuccess)
        {
            return Result<bool>.Failure(
                idResult.ErrorType,
                idResult.Errors);
        }

        EmployeeId idToCheck = idResult.Value!;

        bool exists = await _employeeRepository.EmployeeExistsWithSameCombination(
            idToCheck,
            employeeNumber,
            personName,
            emailAddress,
            establishmentId);

        if (exists)
        {
            return Result<bool>.Failure(
                ErrorType.Conflict,
                new Error(EmployeeException.DuplicateResource.Code.ToString(),
                    EmployeeException.DuplicateEmployeeErrorMessage));
        }

        return Result<bool>.Success(false);
    }

    public async Task<Result<EmployeeResponse>> GetEmployeeByIdAsync(EmployeeId employeeId)
    {
        Result<Employee> employeeResult = await _employeeRepository.GetByIdAsync(employeeId);
        return employeeResult.IsSuccess
            ? Result<EmployeeResponse>.Success(employeeResult.Value!.ToEmployeeResponse())
            : Result<EmployeeResponse>.Failure(employeeResult.ErrorType, employeeResult.Errors);
    }

    public async Task<Result<IEnumerable<EmployeeResponse>>> GetAllEmployeesAsync(EmployeeSpecification spec)
    {
        Result<IEnumerable<Employee>> employeesResult = await _employeeRepository.ListAsync(spec);
        return employeesResult.IsSuccess
            ? Result<IEnumerable<EmployeeResponse>>.Success(employeesResult.Value!.Select(e => e.ToEmployeeResponse()))
            : Result<IEnumerable<EmployeeResponse>>.Failure(employeesResult.ErrorType, employeesResult.Errors);
    }

    public async Task<Result<int>> CountAsync(EmployeeSpecification spec)
        => await _employeeRepository.CountAsync(spec);
}
