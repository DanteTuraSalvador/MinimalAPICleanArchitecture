using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEstablishmentContactService
{
    Task<Result<EstablishmentContact>> CreateEstablishmentContactAsync(EstablishmentContactForCreationRequest creationRequest);

    Task<Result<EstablishmentContact>> GetEstablishmentContactByIdAsync(EstablishmentContactId establishmentContactId);

    Task<Result<IEnumerable<EstablishmentContact>>> GetEstablishmentContactsAsync(ISpecification<EstablishmentContact> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentContact> spec);

    Task<Result<EstablishmentContact>> UpdateEstablishmentContactAsync(EstablishmentContactId establishmentContactId, EstablishmentContactForUpdateRequest updateRequest);

    Task<Result<EstablishmentContact>> PatchEstablishmentContactAsync(EstablishmentContactId establishmentContactId, EstablishmentContactPatchRequest patchRequest);

    Task<Result> DeleteEstablishmentContactAsync(EstablishmentContactId establishmentContactId);

    Task<Result<bool>> EstablishmentContactCombinationExistsAsync(
      PersonName contactPerson,
      PhoneNumber contactPhoneNumber,
      EstablishmentId establishmentId,
      EstablishmentContactId? excludedContactId = null);
}
