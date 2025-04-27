using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Persistence;

//public interface IEstablishmentAddressRepository
//{
//    Task<Result<EstablishmentAddress>> GetByIdAsync(EstablishmentAddressId establishmentAddressId, CancellationToken cancellationToken = default);

//    Task<Result> SetNonPrimaryForEstablishmentAsync(EstablishmentId establishmentId, EstablishmentAddressId excludedAddressId, CancellationToken cancellationToken = default);

//    Task<Result<EstablishmentAddress>> AddAsync(EstablishmentAddress establishmentAddress, CancellationToken cancellationToken = default);

//    Task<bool> ExistsAsync(EstablishmentAddressId id, CancellationToken cancellationToken = default);

//    Task DetachAsync(EstablishmentAddress establishmentAddress);

//    Task<Result<EstablishmentAddress>> UpdateAsync(EstablishmentAddress establishmentAddress, CancellationToken cancellationToken = default);

//    Task<Result> DeleteAsync(EstablishmentAddressId id, CancellationToken cancellationToken = default);

//    Task<Result<IEnumerable<EstablishmentAddress>>> ListAsync(ISpecification<EstablishmentAddress> spec);

//    Task<Result<int>> CountAsync(ISpecification<EstablishmentAddress> spec);

//    Task<bool> AddressExistsWithSameCoordinatesInEstablishment(
//        EstablishmentAddressId excludedId,
//        decimal latitude,
//        decimal longitude,
//        EstablishmentId establishmentId);
//}

public interface IEstablishmentAddressRepository : IGenericRepository<EstablishmentAddress, EstablishmentAddressId>
{
    Task<Result> SetNonPrimaryForEstablishmentAsync(EstablishmentId establishmentId, EstablishmentAddressId excludedAddressId, CancellationToken cancellationToken = default);

    Task DetachAsync(EstablishmentAddress establishmentAddress);

    Task<Result<IEnumerable<EstablishmentAddress>>> ListAsync(ISpecification<EstablishmentAddress> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentAddress> spec);

    Task<bool> AddressExistsWithSameCoordinatesInEstablishment(
        EstablishmentAddressId excludedId,
        decimal latitude,
        decimal longitude,
        EstablishmentId establishmentId);

    Task<bool> ExistsAsync(EstablishmentAddressId id);
}
