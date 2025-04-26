using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.Domain.Employees;

public sealed class Employee : AggregateRoot<EmployeeId>
{
    public EmployeeId EmployeeId => Id;
    public EmployeeNumber EmployeeNumber { get; private set; }
    public PersonName EmployeeName { get; private set; }
    public EmailAddress EmployeeEmail { get; private set; }
    public EmployeeStatus EmployeeStatus { get; private set; }
    public EmployeeRoleId EmployeeRoleId { get; private set; }
    public EstablishmentId EstablishmentId { get; private set; }

    public bool IsEmpty() => this == Empty();

    private static readonly Lazy<Employee> _empty = new(() => new Employee());

    public static Employee Empty() => _empty.Value;

    public Establishment Establishment { get; private set; } = default!;
    public EmployeeRole EmployeeRole { get; private set; } = default!;

    private Employee() : base(EmployeeId.Empty())
        => (EmployeeNumber, EmployeeName, EmployeeEmail, EmployeeStatus, EmployeeRoleId, EstablishmentId)
            = (EmployeeNumber.Empty(), PersonName.Empty(), EmailAddress.Empty(), EmployeeStatus.None, EmployeeRoleId.Empty(), EstablishmentId.Empty());

    private Employee(EmployeeId employeeId, EmployeeNumber employeeNumber, PersonName employeeName, EmailAddress employeeEmail, EmployeeStatus employeeStatus, EmployeeRoleId employeeRoleId, EstablishmentId establishmentId) : base(employeeId)
            => (EmployeeNumber, EmployeeName, EmployeeEmail, EmployeeStatus, EmployeeRoleId, EstablishmentId)
                = (employeeNumber, employeeName, employeeEmail, employeeStatus, employeeRoleId, establishmentId);

    public static Result<Employee> Create(EmployeeNumber employeeNumber, PersonName employeeName, EmailAddress email, EmployeeRoleId employeeRoleId, EstablishmentId establishmentId)
    {
        Result<EmployeeNumber> employeeNumberResult = EmployeeNumber.Create(employeeNumber.EmployeeNo);
        Result<PersonName> personNameResult = PersonName.Create(employeeName.FirstName, employeeName.MiddleName, employeeName.LastName);
        Result<EmailAddress> emailResult = EmailAddress.Create(email.Email);

        var result = Result.Combine(
            employeeNumberResult.ToResult(),
            personNameResult.ToResult(),
            emailResult.ToResult(),
            Guard.AgainstCondition(employeeRoleId == EmployeeRoleId.Empty(), static () => EmployeeRoleException.NullEmployeeRole()),
            Guard.AgainstCondition(establishmentId == EstablishmentId.Empty(), static () => StronglyTypedIdException.NullId())
        );

        return result.IsSuccess
            ? Result<Employee>.Success(new Employee(EmployeeId.New(), employeeNumberResult.Value!, personNameResult.Value!, emailResult.Value!, EmployeeStatus.Active, employeeRoleId, establishmentId))
            : Result<Employee>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<Employee> WithEmployeeNumber(EmployeeNumber newEmployeeNumber)
    {
        Result<EmployeeNumber> employeeNumberResult = EmployeeNumber.Create(newEmployeeNumber.EmployeeNo);

        return employeeNumberResult.IsSuccess
            ? Result<Employee>.Success(new Employee(EmployeeId, employeeNumberResult.Value!, EmployeeName, EmployeeEmail, EmployeeStatus, EmployeeRoleId, EstablishmentId))
            : Result<Employee>.Failure(ErrorType.Validation, employeeNumberResult.Errors);
    }

    public Result<Employee> WithPersonName(PersonName newEmployeeName)
    {
        Result<PersonName> personNameResult = PersonName.Create(newEmployeeName.FirstName, newEmployeeName.MiddleName, newEmployeeName.LastName);

        return personNameResult.IsSuccess
            ? Result<Employee>.Success(new Employee(EmployeeId, EmployeeNumber, personNameResult.Value!, EmployeeEmail, EmployeeStatus, EmployeeRoleId, EstablishmentId))
            : Result<Employee>.Failure(ErrorType.Validation, personNameResult.Errors);
    }

    public Result<Employee> WithEmail(EmailAddress newEmail)
    {
        Result<EmailAddress> emailResult = EmailAddress.Create(newEmail.Email);

        return emailResult.IsSuccess
            ? Result<Employee>.Success(new Employee(EmployeeId, EmployeeNumber, EmployeeName, emailResult.Value!, EmployeeStatus, EmployeeRoleId, EstablishmentId))
            : Result<Employee>.Failure(ErrorType.Validation, emailResult.Errors);
    }

    public Result<Employee> WithEmployeeStatus(EmployeeStatus newEmployeeStatus)
    {
        Result transitionResult = EmployeeStatus.ValidateTransition(EmployeeStatus, newEmployeeStatus);

        if (!transitionResult.IsSuccess)
        {
            return Result<Employee>.Failure(ErrorType.Validation, transitionResult.Errors);
        }

        return Result<Employee>.Success(new Employee(EmployeeId, EmployeeNumber, EmployeeName, EmployeeEmail, newEmployeeStatus, EmployeeRoleId, EstablishmentId));
    }

    public Result<Employee> WithEmployeeRole(EmployeeRoleId newRole)
    {
        Result result = Guard.AgainstCondition(newRole == EmployeeRoleId.Empty(), static () => EmployeeRoleException.NullEmployeeRole());

        return result.IsSuccess
            ? Result<Employee>.Success(new Employee(EmployeeId, EmployeeNumber, EmployeeName, EmployeeEmail, EmployeeStatus, newRole, EstablishmentId))
            : Result<Employee>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<Employee> WithEstablishmentId(EstablishmentId newEstablishmentId)
    {
        Result result = Guard.AgainstCondition(newEstablishmentId == EstablishmentId.Empty(), static () => StronglyTypedIdException.NullId());

        return result.IsSuccess
            ? Result<Employee>.Success(new Employee(EmployeeId, EmployeeNumber, EmployeeName, EmployeeEmail, EmployeeStatus, EmployeeRoleId, newEstablishmentId))
            : Result<Employee>.Failure(ErrorType.Validation, result.Errors);
    }

    public override string ToString() => $"{EmployeeName} - {EmployeeEmail} - {EmployeeStatus} - {EmployeeRoleId} - {EstablishmentId}";
}
