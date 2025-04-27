using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEstablishmentPhoneService
{
    Task<Result<EstablishmentPhoneResponse>> GetEstablishmentPhoneByIdAsync(EstablishmentPhoneId establishmentPhoneId);

    Task<Result<EstablishmentPhoneResponse>> CreateEstablishmentPhoneAsync(EstablishmentPhoneForCreationRequest establishmentPhoneForCreationRequest);

    Task<Result<EstablishmentPhoneResponse>> UpdateEstablishmentPhoneAsync(EstablishmentPhoneId establishmentPhoneId, EstablishmentPhoneForUpdateRequest establishmentPhoneForUpdateRequest);

    Task<Result<EstablishmentPhoneResponse>> PatchEstablishmentPhoneAsync(EstablishmentPhoneId establishmentPhoneId, EstablishmentPhonePatchRequest establishmentPhonePatchRequest);

    Task<Result> DeleteEstablishmentPhoneAsync(EstablishmentPhoneId establishmentPhoneId);

    Task<Result<IEnumerable<EstablishmentPhoneResponse>>> ListAsync(ISpecification<EstablishmentPhone> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentPhone> spec);

    Task<Result<bool>> EstablishmentPhoneCombinationExistsAsync(
        PhoneNumber phoneNumber,
        EstablishmentId establishmentId,
        EstablishmentPhoneId? excludedPhoneId = null);
}
