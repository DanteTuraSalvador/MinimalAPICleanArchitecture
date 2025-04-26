using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Application.Contracts.Interfaces.Persistence;

//public interface IEstablishmentMemberRepository
//{
//    Task<Result<EstablishmentMember>> GetByIdAsync(EstablishmentMemberId establishmentMemberId, CancellationToken cancellationToken = default);

//    Task<Result<EstablishmentMember>> AddAsync(EstablishmentMember establishmentMember, CancellationToken cancellationToken = default);

//    Task<bool> ExistsAsync(EstablishmentMemberId id, CancellationToken cancellationToken = default);

//    Task DetachAsync(EstablishmentMember establishmentMember);

//    Task<Result<EstablishmentMember>> UpdateAsync(EstablishmentMember establishmentMember, CancellationToken cancellationToken = default);

//    Task<Result> DeleteAsync(EstablishmentMemberId id, CancellationToken cancellationToken = default);

//    Task<Result<IEnumerable<EstablishmentMember>>> ListAsync(ISpecification<EstablishmentMember> spec);

//    Task<Result<int>> CountAsync(ISpecification<EstablishmentMember> spec);

//    Task<bool> MemberExistsForEmployeeInEstablishment(
//        EstablishmentMemberId? excludedId,
//        EmployeeId employeeId,
//        EstablishmentId establishmentId);
//}


public interface IEstablishmentMemberRepository : IGenericRepository<EstablishmentMember, EstablishmentMemberId>
{
    Task DetachAsync(EstablishmentMember establishmentMember);

    Task<Result<IEnumerable<EstablishmentMember>>> ListAsync(ISpecification<EstablishmentMember> spec);

    Task<Result<int>> CountAsync(ISpecification<EstablishmentMember> spec);

    Task<bool> MemberExistsForEmployeeInEstablishment(
        EstablishmentMemberId? excludedId,
        EmployeeId employeeId,
        EstablishmentId establishmentId);

    Task<bool> ExistsAsync(EstablishmentMemberId id);
}
