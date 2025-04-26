using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEstablishmentService
{
    Task<Result<Establishment>> CreateEstablishmentAsync(
        EstablishmentForCreationRequest establishmentForCreationRequest);

    Task<Result<Establishment>> UpdateEstablishmentAsync(
        EstablishmentId establishmentId,
        EstablishmentForUpdateRequest establishmentForUpdateRequest);

    Task<Result> DeleteEstablishmentAsync(
        EstablishmentId establishmentId);

    Task<Result<Establishment>> PatchEstablishmentAsync(
        EstablishmentId establishmentId,
        EstablishmentPatchRequest establishmentPatchRequest);

    Task<Result<Establishment>> GetEstablishmentByIdAsync(EstablishmentId establishmentId);

    Task<Result<int>> CountAsync(ISpecification<Establishment> spec);

    Task<Result<IEnumerable<Establishment>>> GetEstablishmentsAsync(ISpecification<Establishment> spec);
}
