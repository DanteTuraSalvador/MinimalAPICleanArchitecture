using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Contracts.Interfaces.Persistence;

//public interface IEstablishmentContactRepository
//{
//    Task<Result<EstablishmentContact>> GetByIdAsync(EstablishmentContactId establishmentContactId, CancellationToken cancellationToken = default);

//    Task<Result<EstablishmentContact>> AddAsync(EstablishmentContact establishmentContact, CancellationToken cancellationToken = default);

//    Task<Result<EstablishmentContact>> UpdateAsync(EstablishmentContact establishmentContact, CancellationToken cancellationToken = default);

//    Task<Result> DeleteAsync(EstablishmentContactId id, CancellationToken cancellationToken = default);

//    Task<bool> ExistsAsync(EstablishmentContactId id, CancellationToken cancellationToken = default);

//    Task DetachAsync(EstablishmentContact establishmentContact);

//    Task<Result<IEnumerable<EstablishmentContact>>> ListAsync(ISpecification<EstablishmentContact> spec);

//    Task<Result<int>> CountAsync(ISpecification<EstablishmentContact> spec);

//    Task<Result> SetNonPrimaryForEstablishmentContanctAsync(EstablishmentId establishmentId, EstablishmentContactId excludeEstablishmentContactId, CancellationToken cancellationToken = default);

//    Task<bool> ContactExistsWithSameDetailsInEstablishment(
//        EstablishmentContactId excludedId,
//        PersonName contactPerson,
//        PhoneNumber contactPhoneNumber,
//        EstablishmentId establishmentId);
//}



public interface IEstablishmentContactRepository : IGenericRepository<EstablishmentContact, EstablishmentContactId>
{
    Task<Result> SetNonPrimaryForEstablishmentContanctAsync(EstablishmentId establishmentId, EstablishmentContactId excludeEstablishmentContactId);

    Task DetachAsync(EstablishmentContact establishmentContact);

    Task<Result<IEnumerable<EstablishmentContact>>> ListAsync(ISpecification<EstablishmentContact> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentContact> spec);

    Task<bool> ContactExistsWithSameDetailsInEstablishment(
        EstablishmentContactId excludedId,
        PersonName contactPerson,
        PhoneNumber contactPhoneNumber,
        EstablishmentId establishmentId);

    Task<bool> ExistsAsync(EstablishmentContactId id);
}
