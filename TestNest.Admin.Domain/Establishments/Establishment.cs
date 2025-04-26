using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;
using TestNest.Admin.SharedLibrary.ValueObjects.Enums;

namespace TestNest.Admin.Domain.Establishments;

public class Establishment : AggregateRoot<EstablishmentId>
{
    public EstablishmentId EstablishmentId => Id;
    public EstablishmentName EstablishmentName { get; private set; }
    public EmailAddress EstablishmentEmail { get; private set; }
    public EstablishmentStatus EstablishmentStatus { get; private set; }

    public bool IsEmpty() => this == Empty();

    private static readonly Lazy<Establishment> _empty = new(() => new Establishment());

    public static Establishment Empty() => _empty.Value;

    private Establishment() : base(EstablishmentId.Empty())
    {
        EstablishmentName = EstablishmentName.Empty();
        EstablishmentEmail = EmailAddress.Empty();
        EstablishmentStatus = EstablishmentStatus.Pending;
    }

    private Establishment(
        EstablishmentId establishmentId,
        EstablishmentName establishmentName,
        EmailAddress establishmentEmail,
        EstablishmentStatus establishmentStatus
    ) : base(establishmentId)
    {
        EstablishmentName = establishmentName;
        EstablishmentEmail = establishmentEmail;
        EstablishmentStatus = establishmentStatus;
    }

    public static Result<Establishment> Create(
        EstablishmentName establishmentName,
        EmailAddress establishmentEmail)
    {
        Result<EstablishmentName> establishmentNameResult = EstablishmentName.Create(establishmentName.Name);
        Result<EmailAddress> establishmentEmailResult = EmailAddress.Create(establishmentEmail.Email);

        var combinedValidationResult = Result.Combine(
            establishmentNameResult.ToResult(),
            establishmentEmailResult.ToResult()
        );

        return combinedValidationResult.IsSuccess
            ? Result<Establishment>.Success(new Establishment(
                EstablishmentId.New(),
                establishmentNameResult.Value!,
                establishmentEmailResult.Value!,
                EstablishmentStatus.Pending))
            : Result<Establishment>.Failure(ErrorType.Validation, combinedValidationResult.Errors);
    }

    public Result<Establishment> WithStatus(EstablishmentStatus newStatus)
    {
        Result result = Guard.AgainstNull(newStatus, static () => EstablishmentStatusException.NullStatus());
        Result<EstablishmentStatus> establishmentStatusResult = EstablishmentStatus.FromId(newStatus.Id);
        var combinedValidationResult = Result.Combine(result, establishmentStatusResult.ToResult());

        if (!combinedValidationResult.IsSuccess)
        {
            return Result<Establishment>.Failure(ErrorType.Validation, combinedValidationResult.Errors);
        }

        Result transitionResult = EstablishmentStatus.ValidateTransition(EstablishmentStatus, newStatus);

        if (!transitionResult.IsSuccess)
        {
            return Result<Establishment>.Failure(ErrorType.Validation, transitionResult.Errors);
        }

        return Result<Establishment>.Success(new Establishment(
            EstablishmentId,
            EstablishmentName,
            EstablishmentEmail,
            newStatus));
    }

    public Result<Establishment> WithName(EstablishmentName newName)
    {
        Result<EstablishmentName> establishmentNameResult = EstablishmentName.Create(newName.Name);

        return establishmentNameResult.IsSuccess
            ? Result<Establishment>.Success(new Establishment(
                EstablishmentId,
                establishmentNameResult.Value!,
                EstablishmentEmail,
                EstablishmentStatus))
            : Result<Establishment>.Failure(ErrorType.Validation, establishmentNameResult.Errors);
    }

    public Result<Establishment> WithEmail(EmailAddress newEmail)
    {
        Result<EmailAddress> emailResult = EmailAddress.Create(newEmail.Email);

        return emailResult.IsSuccess
            ? Result<Establishment>.Success(new Establishment(
                EstablishmentId,
                EstablishmentName,
                newEmail,
                EstablishmentStatus))
            : Result<Establishment>.Failure(
                ErrorType.Validation,
                emailResult.Errors);
    }
}
