using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Domain.Establishments;

public sealed class EstablishmentPhone : BaseEntity<EstablishmentPhoneId>
{
    public EstablishmentPhoneId EstablishmentPhoneId => Id;

    public EstablishmentId EstablishmentId { get; private set; }
    public PhoneNumber EstablishmentPhoneNumber { get; private set; }
    public bool IsPrimary { get; private set; }

    private static readonly Lazy<EstablishmentPhone> _empty = new(()
        => new EstablishmentPhone());

    public static EstablishmentPhone Empty()
        => _empty.Value;

    public Establishment Establishment { get; private set; } = default!;

    private EstablishmentPhone(
        EstablishmentPhoneId establishmentPhoneId,
        EstablishmentId establishmentId,
        PhoneNumber establishmentPhoneNumber,
        bool isPrimary) : base(establishmentPhoneId)
            => (EstablishmentId, EstablishmentPhoneNumber, IsPrimary)
                = (establishmentId, establishmentPhoneNumber, isPrimary);

    private EstablishmentPhone() : base(EstablishmentPhoneId.New())
        => (EstablishmentId, EstablishmentPhoneNumber, IsPrimary)
            = (EstablishmentId.Empty(), PhoneNumber.Empty(), false);

    public static Result<EstablishmentPhone> Create(
        EstablishmentId establishmentId,
        PhoneNumber phoneNumber,
        bool isPrimary = true)
    {
        var result = Result.Combine(
            Guard.AgainstCondition(establishmentId == EstablishmentId.Empty(),
               static () => StronglyTypedIdException.NullId()),
            Guard.AgainstCondition(phoneNumber.IsEmpty(),
               static () => PhoneNumberException.EmptyPhoneNumber())
        );

        return result.IsSuccess
            ? Result<EstablishmentPhone>.Success(new EstablishmentPhone(
                EstablishmentPhoneId.New(),
                establishmentId,
                phoneNumber,
                isPrimary))
            : Result<EstablishmentPhone>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentPhone> WithPhoneNumber(PhoneNumber newPhoneNumber)
    {
        Result result = Guard.AgainstCondition(newPhoneNumber.IsEmpty(),
            static () => PhoneNumberException.EmptyPhoneNumber());

        return result.IsSuccess
            ? Result<EstablishmentPhone>.Success(new EstablishmentPhone(
                EstablishmentPhoneId,
                EstablishmentId,
                newPhoneNumber,
                IsPrimary))
            : Result<EstablishmentPhone>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentPhone> WithIsPrimary(bool isPrimary)
        => Result<EstablishmentPhone>.Success(
            new EstablishmentPhone(
                EstablishmentPhoneId,
                EstablishmentId,
                EstablishmentPhoneNumber,
                isPrimary)
        );
}
