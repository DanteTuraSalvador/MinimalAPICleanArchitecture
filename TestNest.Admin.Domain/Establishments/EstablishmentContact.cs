using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Domain.Establishments;

public sealed class EstablishmentContact : BaseEntity<EstablishmentContactId>
{
    public EstablishmentContactId EstablishmentContactId => Id;
    public EstablishmentId EstablishmentId { get; private set; }
    public PersonName ContactPerson { get; private set; }
    public PhoneNumber ContactPhone { get; private set; }
    public bool IsPrimary { get; private set; }

    private static readonly Lazy<EstablishmentContact> _empty = new(()
        => new EstablishmentContact());

    public static EstablishmentContact Empty() => _empty.Value;

    public Establishment Establishment { get; private set; } = default!;

    private EstablishmentContact() : base(EstablishmentContactId.Empty())
        => (EstablishmentId, ContactPerson, ContactPhone, IsPrimary)
            = (EstablishmentId.Empty(), PersonName.Empty(), PhoneNumber.Empty(), false);

    private EstablishmentContact(
        EstablishmentContactId establishmentContactId,
        EstablishmentId establishmentId,
        PersonName contactPerson,
        PhoneNumber contactPhone,
        bool isPrimary) : base(establishmentContactId)
        => (EstablishmentId, ContactPerson, ContactPhone, IsPrimary)
            = (establishmentId, contactPerson, contactPhone, isPrimary);

    public static Result<EstablishmentContact> Create(
       EstablishmentId establishmentId,
       PersonName contactPerson,
       PhoneNumber contactPhone,
       bool isPrimary = true)
    {
        var result = Result.Combine(
            Guard.AgainstCondition(contactPerson.IsEmpty(),
                static () => PersonNameException.NullPersonName()),
            Guard.AgainstCondition(contactPhone.IsEmpty(),
                static () => PhoneNumberException.InvalidFormat())
        );

        return result.IsSuccess
            ? Result<EstablishmentContact>.Success(new EstablishmentContact(
                EstablishmentContactId.New(),
                establishmentId,
                contactPerson,
                contactPhone,
                isPrimary))
            : Result<EstablishmentContact>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentContact> WithContactPerson(PersonName newContactPerson)
    {
        Result result = Guard.AgainstCondition(newContactPerson.IsEmpty(),
            static () => PersonNameException.NullPersonName());

        return result.IsSuccess
            ? Result<EstablishmentContact>.Success(new EstablishmentContact(
                EstablishmentContactId,
                EstablishmentId,
                newContactPerson,
                ContactPhone,
                IsPrimary))
            : Result<EstablishmentContact>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentContact> WithContactPhone(PhoneNumber newPhone)
    {
        Result result = Guard.AgainstCondition(newPhone.IsEmpty(),
            static () => PhoneNumberException.InvalidFormat());

        return result.IsSuccess
            ? Result<EstablishmentContact>.Success(new EstablishmentContact(
                EstablishmentContactId,
                EstablishmentId,
                ContactPerson,
                newPhone,
                IsPrimary))
            : Result<EstablishmentContact>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentContact> WithPrimaryFlag(bool isPrimary)
        => Result<EstablishmentContact>.Success(new EstablishmentContact(
            EstablishmentContactId,
            EstablishmentId,
            ContactPerson,
            ContactPhone,
            isPrimary));
}
