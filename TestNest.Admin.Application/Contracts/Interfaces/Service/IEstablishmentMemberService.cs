using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Dtos.Requests.Establishment;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Service;

public interface IEstablishmentMemberService
{
    Task<Result<EstablishmentMember>> GetEstablishmentMemberByIdAsync(EstablishmentMemberId establishmentMemberId);

    Task<Result<EstablishmentMember>> CreateEstablishmentMemberAsync(EstablishmentMemberForCreationRequest establishmentMemberForCreationRequest);

    Task<Result<EstablishmentMember>> UpdateEstablishmentMemberAsync(EstablishmentMemberId establishmentMemberId, EstablishmentMemberForUpdateRequest establishmentMemberForUpdateRequest);

    Task<Result<EstablishmentMember>> PatchEstablishmentMemberAsync(EstablishmentMemberId establishmentMemberId, EstablishmentMemberPatchRequest establishmentMemberPatchRequest);

    Task<Result> DeleteEstablishmentMemberAsync(EstablishmentMemberId establishmentMemberId);

    Task<Result<IEnumerable<EstablishmentMember>>> ListAsync(ISpecification<EstablishmentMember> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentMember> spec);

    // You might need a method to check if a member with the same employee ID
    // already exists within the same establishment.
    Task<Result<bool>> EstablishmentMemberWithEmployeeExistsAsync(
        EmployeeId employeeId,
        EstablishmentId establishmentId,
        EstablishmentMemberId? excludedMemberId = null);
}
