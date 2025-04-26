using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Persistence;

//public interface IEstablishmentPhoneRepository
//{
//    Task<Result<EstablishmentPhone>> GetByIdAsync(EstablishmentPhoneId establishmentPhoneId, CancellationToken cancellationToken = default);

//    Task<Result> SetNonPrimaryForEstablishmentAsync(EstablishmentId establishmentId, EstablishmentPhoneId excludedPhoneId, CancellationToken cancellationToken = default);

//    Task<Result<EstablishmentPhone>> AddAsync(EstablishmentPhone establishmentPhone, CancellationToken cancellationToken = default);

//    Task<bool> ExistsAsync(EstablishmentPhoneId id, CancellationToken cancellationToken = default);

//    Task DetachAsync(EstablishmentPhone establishmentPhone);

//    Task<Result<EstablishmentPhone>> UpdateAsync(EstablishmentPhone establishmentPhone, CancellationToken cancellationToken = default);

//    Task<Result> DeleteAsync(EstablishmentPhoneId id, CancellationToken cancellationToken = default);

//    Task<Result<IEnumerable<EstablishmentPhone>>> ListAsync(ISpecification<EstablishmentPhone> spec);

//    Task<Result<int>> CountAsync(ISpecification<EstablishmentPhone> spec);

//    Task<bool> PhoneExistsWithSameNumberInEstablishment(
//        EstablishmentPhoneId excludedId,
//        string phoneNumber,
//        EstablishmentId establishmentId);
//}
public interface IEstablishmentPhoneRepository : IGenericRepository<EstablishmentPhone, EstablishmentPhoneId>
{
    Task<Result> SetNonPrimaryForEstablishmentAsync(EstablishmentId establishmentId, EstablishmentPhoneId excludedPhoneId);

    Task DetachAsync(EstablishmentPhone establishmentPhone);

    Task<Result<IEnumerable<EstablishmentPhone>>> ListAsync(ISpecification<EstablishmentPhone> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentPhone> spec);

    Task<bool> PhoneExistsWithSameNumberInEstablishment(
        EstablishmentPhoneId excludedId,
        string phoneNumber,
        EstablishmentId establishmentId);

    Task<bool> ExistsAsync(EstablishmentPhoneId id);
}
