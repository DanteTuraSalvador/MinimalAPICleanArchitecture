using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEstablishmentService
{
    Task<Result<EstablishmentResponse>> CreateEstablishmentAsync(
        EstablishmentForCreationRequest establishmentForCreationRequest);

    Task<Result<EstablishmentResponse>> UpdateEstablishmentAsync(
        EstablishmentId establishmentId,
        EstablishmentForUpdateRequest establishmentForUpdateRequest);

    Task<Result> DeleteEstablishmentAsync(
        EstablishmentId establishmentId);

    Task<Result<EstablishmentResponse>> PatchEstablishmentAsync(
        EstablishmentId establishmentId,
        EstablishmentPatchRequest establishmentPatchRequest);

    Task<Result<EstablishmentResponse>> GetEstablishmentByIdAsync(EstablishmentId establishmentId); // Updated return type

    Task<Result<int>> CountAsync(ISpecification<Establishment> spec);

    Task<Result<IEnumerable<EstablishmentResponse>>> GetEstablishmentsAsync(ISpecification<Establishment> spec); // Updated return type
}
