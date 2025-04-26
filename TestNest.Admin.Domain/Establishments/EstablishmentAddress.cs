using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Guards;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Domain.Establishments;

public sealed class EstablishmentAddress : BaseEntity<EstablishmentAddressId>
{
    public EstablishmentAddressId EstablishmentAddressId => Id;
    public EstablishmentId EstablishmentId { get; private set; }
    public Address Address { get; private set; }
    public bool IsPrimary { get; internal set; }

    private static readonly Lazy<EstablishmentAddress> _empty
        = new(() => new EstablishmentAddress());

    public static EstablishmentAddress Empty()
        => _empty.Value;

    public Establishment Establishment { get; private set; } = default!;

    private EstablishmentAddress() : base(EstablishmentAddressId.Empty())
        => (EstablishmentId, Address, IsPrimary)
            = (EstablishmentId.Empty(), Address.Empty(), false);

    private EstablishmentAddress(
        EstablishmentAddressId establishmentAddressId,
        EstablishmentId establishmentId,
        Address address,
        bool isPrimary) : base(establishmentAddressId)
        => (EstablishmentId, Address, IsPrimary)
            = (establishmentId, address, isPrimary);

    public static Result<EstablishmentAddress> Create(
        EstablishmentId establishmentId,
        Address address,
        bool isPrimary = true)
    {
        var result = Result.Combine(
             Guard.AgainstCondition(establishmentId == EstablishmentId.Empty(),
                static () => StronglyTypedIdException.NullId()),
             Guard.AgainstCondition(address.IsEmpty(),
                static () => AddressException.NullAddress())
        );

        return result.IsSuccess
            ? Result<EstablishmentAddress>.Success(new EstablishmentAddress(
                EstablishmentAddressId.New(),
                establishmentId,
                address,
                isPrimary))
            : Result<EstablishmentAddress>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentAddress> WithAddress(Address newAddress)
    {
        Result result = Guard.AgainstCondition(newAddress.IsEmpty(),
            static () => AddressException.NullAddress());

        return result.IsSuccess
            ? Result<EstablishmentAddress>.Success(new EstablishmentAddress(
                EstablishmentAddressId,
                EstablishmentId,
                newAddress,
                IsPrimary))
            : Result<EstablishmentAddress>.Failure(ErrorType.Validation, result.Errors);
    }

    public Result<EstablishmentAddress> WithIsPrimary(bool isPrimary)
         => Result<EstablishmentAddress>.Success(new EstablishmentAddress(
            EstablishmentAddressId,
            EstablishmentId,
            Address,
            isPrimary));

    public Result<EstablishmentAddress> WithDetails(
        Address newAddress,
        bool isPrimary)
    {
        Result addressResult = Guard.AgainstCondition(newAddress.IsEmpty(), static () => AddressException.NullAddress());

        return addressResult.IsSuccess
            ? Result<EstablishmentAddress>.Success(new EstablishmentAddress(
                Id,
                EstablishmentId,
                newAddress,
                isPrimary))
            : Result<EstablishmentAddress>.Failure(ErrorType.Validation, addressResult.Errors);
    }

    public override string ToString() => $"{Address.AddressLine}, {Address.Municipality}" +
                                         $", {Address.City}, {Address.Province}, {Address.Region}, {Address.Country}";
}
