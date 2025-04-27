using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.Dtos.Responses.Establishments;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEstablishmentMemberService
{
    Task<Result<EstablishmentMemberResponse>> GetEstablishmentMemberByIdAsync(EstablishmentMemberId establishmentMemberId);

    Task<Result<EstablishmentMemberResponse>> CreateEstablishmentMemberAsync(EstablishmentMemberForCreationRequest establishmentMemberForCreationRequest);

    Task<Result<EstablishmentMemberResponse>> UpdateEstablishmentMemberAsync(EstablishmentMemberId establishmentMemberId, EstablishmentMemberForUpdateRequest establishmentMemberForUpdateRequest);

    Task<Result<EstablishmentMemberResponse>> PatchEstablishmentMemberAsync(EstablishmentMemberId establishmentMemberId, EstablishmentMemberPatchRequest establishmentMemberPatchRequest);

    Task<Result> DeleteEstablishmentMemberAsync(EstablishmentMemberId establishmentMemberId);

    Task<Result<IEnumerable<EstablishmentMemberResponse>>> ListAsync(ISpecification<EstablishmentMember> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentMember> spec);

    Task<Result<bool>> EstablishmentMemberWithEmployeeExistsAsync(
        EmployeeId employeeId,
        EstablishmentId establishmentId,
        EstablishmentMemberId? excludedMemberId = null);
}
