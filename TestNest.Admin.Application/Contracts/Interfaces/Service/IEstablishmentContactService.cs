using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEstablishmentContactService
{
    Task<Result<EstablishmentContactResponse>> CreateEstablishmentContactAsync(EstablishmentContactForCreationRequest creationRequest);

    Task<Result<EstablishmentContactResponse>> GetEstablishmentContactByIdAsync(EstablishmentContactId establishmentContactId);

    Task<Result<IEnumerable<EstablishmentContactResponse>>> GetEstablishmentContactsAsync(ISpecification<EstablishmentContact> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentContact> spec);

    Task<Result<EstablishmentContactResponse>> UpdateEstablishmentContactAsync(EstablishmentContactId establishmentContactId, EstablishmentContactForUpdateRequest updateRequest);

    Task<Result<EstablishmentContactResponse>> PatchEstablishmentContactAsync(EstablishmentContactId establishmentContactId, EstablishmentContactPatchRequest patchRequest);

    Task<Result> DeleteEstablishmentContactAsync(EstablishmentContactId establishmentContactId);

    Task<Result<bool>> EstablishmentContactCombinationExistsAsync(
        PersonName contactPerson,
        PhoneNumber contactPhoneNumber,
        EstablishmentId establishmentId,
        EstablishmentContactId? excludedContactId = null);
}
