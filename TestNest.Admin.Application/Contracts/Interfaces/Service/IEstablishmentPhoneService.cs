using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEstablishmentPhoneService
{
    Task<Result<EstablishmentPhone>> GetEstablishmentPhoneByIdAsync(EstablishmentPhoneId establishmentPhoneId);

    Task<Result<EstablishmentPhone>> CreateEstablishmentPhoneAsync(EstablishmentPhoneForCreationRequest establishmentPhoneForCreationRequest);

    Task<Result<EstablishmentPhone>> UpdateEstablishmentPhoneAsync(EstablishmentPhoneId establishmentPhoneId, EstablishmentPhoneForUpdateRequest establishmentPhoneForUpdateRequest);

    Task<Result<EstablishmentPhone>> PatchEstablishmentPhoneAsync(EstablishmentPhoneId establishmentPhoneId, EstablishmentPhonePatchRequest establishmentPhonePatchRequest);

    Task<Result> DeleteEstablishmentPhoneAsync(EstablishmentPhoneId establishmentPhoneId);

    Task<Result<IEnumerable<EstablishmentPhone>>> ListAsync(ISpecification<EstablishmentPhone> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentPhone> spec);

    Task<Result<bool>> EstablishmentPhoneCombinationExistsAsync(
       PhoneNumber phoneNumber,
       EstablishmentId establishmentId,
       EstablishmentPhoneId? excludedPhoneId = null);
}
