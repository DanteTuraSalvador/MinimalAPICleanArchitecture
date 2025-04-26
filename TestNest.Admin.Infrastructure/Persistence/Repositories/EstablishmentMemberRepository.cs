using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Common;
using TestNest.Admin.Infrastructure.Persistence.Repositories.Common;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;

namespace TestNest.Admin.Infrastructure.Persistence.Repositories;


public class EstablishmentMemberRepository(
    ApplicationDbContext establishmentMemberDbContext,
    ILogger<EstablishmentMemberRepository> logger)
    : GenericRepository<EstablishmentMember, EstablishmentMemberId>(establishmentMemberDbContext), IEstablishmentMemberRepository
{
    private readonly ApplicationDbContext _establishmentMemberDbContext = establishmentMemberDbContext;
    private readonly ILogger<EstablishmentMemberRepository> _logger = logger;

    public override async Task<Result<EstablishmentMember>> GetByIdAsync(EstablishmentMemberId id)
    {
        try
        {
            EstablishmentMember? memberResult = await _establishmentMemberDbContext.EstablishmentMembers
                .Include(x => x.MemberTitle)
                .Include(x => x.MemberDescription)
                .Include(x => x.MemberTag)
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            if (memberResult == null)
            {
                return Result<EstablishmentMember>.Failure(
                    ErrorType.NotFound,
                    new Error("NotFound", $"EstablishmentMember with ID '{id}' not found."));
            }
            return Result<EstablishmentMember>.Success(memberResult);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentMember>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<IEnumerable<EstablishmentMember>>> GetAllAsync()
    {
        try
        {
            List<EstablishmentMember> members = await _establishmentMemberDbContext.EstablishmentMembers 
                .Include(x => x.MemberTitle)
                .Include(x => x.MemberDescription)
                .Include(x => x.MemberTag)
                .ToListAsync();
            return Result<IEnumerable<EstablishmentMember>>.Success(members);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EstablishmentMember>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<EstablishmentMember>> AddAsync(EstablishmentMember entity)
    {
        try
        {
            _ = await _establishmentMemberDbContext.EstablishmentMembers.AddAsync(entity); 
            return Result<EstablishmentMember>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentMember>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<EstablishmentMember>> UpdateAsync(EstablishmentMember entity)
    {
        try
        {
            _ = _establishmentMemberDbContext.EstablishmentMembers.Update(entity);
            return Result<EstablishmentMember>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentMember>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result> DeleteAsync(EstablishmentMemberId id)
    {
        try
        {
            EstablishmentMember? memberToDelete = await _establishmentMemberDbContext.EstablishmentMembers 
                .FindAsync(id.Value);
            if (memberToDelete != null)
            {
                _ = _establishmentMemberDbContext.EstablishmentMembers.Remove(memberToDelete);
                return Result.Success();
            }
            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"EstablishmentMember with ID '{id}' not found."));
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> ExistsAsync(EstablishmentMemberId id)
        => await _establishmentMemberDbContext.EstablishmentMembers.AnyAsync(e => e.Id == id); 

    public Task DetachAsync(EstablishmentMember establishmentMember)
    {
        _establishmentMemberDbContext.Entry(establishmentMember).State = EntityState.Detached;
        return Task.CompletedTask;
    }

    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentMember> spec)
    {
        try
        {
            IQueryable<EstablishmentMember> query = SpecificationEvaluator<EstablishmentMember>
                .GetQuery(_establishmentMemberDbContext.EstablishmentMembers, (BaseSpecification<EstablishmentMember>)spec); 
            int count = await query.CountAsync();
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<IEnumerable<EstablishmentMember>>> ListAsync(ISpecification<EstablishmentMember> spec)
    {
        try
        {
            IQueryable<EstablishmentMember> query = _establishmentMemberDbContext.EstablishmentMembers 
                .Include(x => x.MemberTitle)
                .Include(x => x.MemberDescription)
                .Include(x => x.MemberTag)
                .AsQueryable();
            var establishmentMemberSpec = (BaseSpecification<EstablishmentMember>)spec;
            query = SpecificationEvaluator<EstablishmentMember>.GetQuery(query, establishmentMemberSpec);
            List<EstablishmentMember> establishmentMembers = await query.ToListAsync();
            return Result<IEnumerable<EstablishmentMember>>.Success(establishmentMembers);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EstablishmentMember>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> MemberExistsForEmployeeInEstablishment(
        EstablishmentMemberId? excludedId,
        EmployeeId employeeId,
        EstablishmentId establishmentId)
    {
        try
        {
            return await _establishmentMemberDbContext.EstablishmentMembers 
                .AnyAsync(em => em.EstablishmentId == establishmentId &&
                                 em.EmployeeId == employeeId &&
                                 em.Id != excludedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if member exists for employee {EmployeeId} in establishment {EstablishmentId}", employeeId, establishmentId);
            return false;
        }
    }
}

//public class EstablishmentMemberRepository(
//    ApplicationDbContext establishmentMemberDbContext,
//    ILogger<EstablishmentMemberRepository> logger) : IEstablishmentMemberRepository
//{
//    private readonly ApplicationDbContext _establishmentMemberDbContext = establishmentMemberDbContext;
//    private readonly ILogger<EstablishmentMemberRepository> _logger = logger;

//    public async Task<Result<EstablishmentMember>> GetByIdAsync(
//        EstablishmentMemberId establishmentMemberId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            EstablishmentMember? memberResult = await _establishmentMemberDbContext.Set<EstablishmentMember>()
//                .Include(x => x.MemberTitle)
//                .Include(x => x.MemberDescription)
//                .Include(x => x.MemberTag)
//                .FirstOrDefaultAsync(m => m.Id == establishmentMemberId, cancellationToken);

//            if (memberResult == null)
//            {
//                return Result<EstablishmentMember>.Failure(
//                    ErrorType.NotFound,
//                    new Error("NotFound", $"EstablishmentMember with ID '{establishmentMemberId}' not found."));
//            }
//            return Result<EstablishmentMember>.Success(memberResult);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentMember>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<EstablishmentMember>> AddAsync(
//        EstablishmentMember establishmentMember,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _ = await _establishmentMemberDbContext.EstablishmentMembers.AddAsync(establishmentMember, cancellationToken);
//            return Result<EstablishmentMember>.Success(establishmentMember);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentMember>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<EstablishmentMember>> UpdateAsync(
//        EstablishmentMember establishmentMember,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _ = _establishmentMemberDbContext.EstablishmentMembers.Update(establishmentMember);
//            return Result<EstablishmentMember>.Success(establishmentMember);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentMember>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> DeleteAsync(
//        EstablishmentMemberId id,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            EstablishmentMember? memberToDelete = await _establishmentMemberDbContext.Set<EstablishmentMember>()
//                .FindAsync([id], cancellationToken);

//            if (memberToDelete != null)
//            {
//                _ = _establishmentMemberDbContext.EstablishmentMembers.Remove(memberToDelete);
//                return Result.Success();
//            }
//            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"EstablishmentMember with ID '{id}' not found."));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> ExistsAsync(
//        EstablishmentMemberId id,
//        CancellationToken cancellationToken = default)
//        => await _establishmentMemberDbContext.EstablishmentMembers.AnyAsync(e => e.Id == id, cancellationToken);

//    public Task DetachAsync(EstablishmentMember establishmentMember)
//    {
//        _establishmentMemberDbContext.Entry(establishmentMember).State = EntityState.Detached;
//        return Task.CompletedTask;
//    }

//    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentMember> spec)
//    {
//        try
//        {
//            IQueryable<EstablishmentMember> query = SpecificationEvaluator<EstablishmentMember>
//                .GetQuery(_establishmentMemberDbContext.EstablishmentMembers, (BaseSpecification<EstablishmentMember>)spec);
//            int count = await query.CountAsync();
//            return Result<int>.Success(count);
//        }
//        catch (Exception ex)
//        {
//            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<IEnumerable<EstablishmentMember>>> ListAsync(ISpecification<EstablishmentMember> spec)
//    {
//        try
//        {
//            IQueryable<EstablishmentMember> query = _establishmentMemberDbContext.EstablishmentMembers
//                .Include(x => x.MemberTitle)
//                .Include(x => x.MemberDescription)
//                .Include(x => x.MemberTag)
//                .AsQueryable();
//            var establishmentMemberSpec = (BaseSpecification<EstablishmentMember>)spec;
//            query = SpecificationEvaluator<EstablishmentMember>.GetQuery(query, establishmentMemberSpec);

//            List<EstablishmentMember> establishmentMembers = await query.ToListAsync();
//            return Result<IEnumerable<EstablishmentMember>>.Success(establishmentMembers);
//        }
//        catch (Exception ex)
//        {
//            return Result<IEnumerable<EstablishmentMember>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> MemberExistsForEmployeeInEstablishment(
//        EstablishmentMemberId? excludedId,
//        EmployeeId employeeId,
//        EstablishmentId establishmentId) => await _establishmentMemberDbContext.EstablishmentMembers
//            .AnyAsync(em => em.EstablishmentId == establishmentId &&
//                             em.EmployeeId == employeeId &&
//                             em.Id != excludedId);
//}
