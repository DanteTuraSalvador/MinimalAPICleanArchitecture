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

public class EstablishmentPhoneRepository(
    ApplicationDbContext establishmentPhoneDbContext,
    ILogger<EstablishmentPhoneRepository> logger)
    : GenericRepository<EstablishmentPhone, EstablishmentPhoneId>(establishmentPhoneDbContext), IEstablishmentPhoneRepository
{
    private readonly ApplicationDbContext _establishmentPhoneDbContext = establishmentPhoneDbContext;
    private readonly ILogger<EstablishmentPhoneRepository> _logger = logger;

    public override async Task<Result<EstablishmentPhone>> GetByIdAsync(EstablishmentPhoneId id)
    {
        try
        {
            EstablishmentPhone? phoneResult = await _establishmentPhoneDbContext.EstablishmentPhones
                .Include(x => x.EstablishmentPhoneNumber)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (phoneResult == null)
            {
                return Result<EstablishmentPhone>.Failure(
                    ErrorType.NotFound,
                    new Error("NotFound", $"EstablishmentPhone with ID '{id}' not found."));
            }
            return Result<EstablishmentPhone>.Success(phoneResult);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentPhone>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<IEnumerable<EstablishmentPhone>>> GetAllAsync()
    {
        try
        {
            List<EstablishmentPhone> phones = await _establishmentPhoneDbContext.EstablishmentPhones
                .Include(x => x.EstablishmentPhoneNumber)
                .ToListAsync();
            return Result<IEnumerable<EstablishmentPhone>>.Success(phones);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EstablishmentPhone>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<EstablishmentPhone>> AddAsync(EstablishmentPhone entity)
    {
        try
        {
            _ = await _establishmentPhoneDbContext.EstablishmentPhones.AddAsync(entity);
            return Result<EstablishmentPhone>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentPhone>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<EstablishmentPhone>> UpdateAsync(EstablishmentPhone entity)
    {
        try
        {
            _ = _establishmentPhoneDbContext.EstablishmentPhones.Update(entity);
            return Result<EstablishmentPhone>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentPhone>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result> DeleteAsync(EstablishmentPhoneId id)
    {
        try
        {
            EstablishmentPhone? entityToDelete = await _establishmentPhoneDbContext.EstablishmentPhones
                .FindAsync(id.Value);
            if (entityToDelete != null)
            {
                _ = _establishmentPhoneDbContext.EstablishmentPhones.Remove(entityToDelete);
                return Result.Success();
            }
            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"{nameof(EstablishmentPhone)} with ID '{id}' not found."));
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> ExistsAsync(EstablishmentPhoneId id)
        => await _establishmentPhoneDbContext.EstablishmentPhones.AnyAsync(e => e.Id == id);

    public Task DetachAsync(EstablishmentPhone establishmentPhone)
    {
        _establishmentPhoneDbContext.Entry(establishmentPhone).State = EntityState.Detached;
        return Task.CompletedTask;
    }

    public async Task<Result> SetNonPrimaryForEstablishmentAsync(
        EstablishmentId establishmentId,
        EstablishmentPhoneId excludedPhoneId)
    {
        try
        {
            int affectedRows = await _establishmentPhoneDbContext.EstablishmentPhones
                .Where(p => p.EstablishmentId == establishmentId && p.IsPrimary && p.Id != excludedPhoneId)
                .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.IsPrimary, false));
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentPhone> spec)
    {
        try
        {
            IQueryable<EstablishmentPhone> query = SpecificationEvaluator<EstablishmentPhone>
                .GetQuery(_establishmentPhoneDbContext.EstablishmentPhones, (BaseSpecification<EstablishmentPhone>)spec);
            int count = await query.CountAsync();
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<IEnumerable<EstablishmentPhone>>> ListAsync(ISpecification<EstablishmentPhone> spec)
    {
        try
        {
            IQueryable<EstablishmentPhone> query = _establishmentPhoneDbContext.EstablishmentPhones
                .Include(x => x.EstablishmentPhoneNumber)
                .AsQueryable();
            var establishmentPhoneSpec = (BaseSpecification<EstablishmentPhone>)spec;
            query = SpecificationEvaluator<EstablishmentPhone>.GetQuery(query, establishmentPhoneSpec);
            List<EstablishmentPhone> establishmentPhones = await query.ToListAsync();
            return Result<IEnumerable<EstablishmentPhone>>.Success(establishmentPhones);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EstablishmentPhone>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> PhoneExistsWithSameNumberInEstablishment(
        EstablishmentPhoneId excludedId,
        string phoneNumber,
        EstablishmentId establishmentId)
    {
        try
        {
            return await _establishmentPhoneDbContext.EstablishmentPhones
                .AnyAsync(ep => ep.EstablishmentId == establishmentId &&
                                 ep.EstablishmentPhoneNumber.PhoneNo == phoneNumber &&
                                 ep.Id != excludedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for existing phone number in establishment {EstablishmentId}", establishmentId);
            return false;
        }
    }
}

//public class EstablishmentPhoneRepository(
//    ApplicationDbContext establishmentPhoneDbContext,
//    ILogger<EstablishmentPhoneRepository> logger) : IEstablishmentPhoneRepository
//{
//    private readonly ApplicationDbContext _establishmentPhoneDbContext = establishmentPhoneDbContext;
//    private readonly ILogger<EstablishmentPhoneRepository> _logger = logger;

//    public async Task<Result<EstablishmentPhone>> GetByIdAsync(
//        EstablishmentPhoneId establishmentPhoneId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            EstablishmentPhone? phoneResult = await _establishmentPhoneDbContext.EstablishmentPhones
//                .Include(x => x.EstablishmentPhoneNumber)
//                .FirstOrDefaultAsync(p => p.Id == establishmentPhoneId, cancellationToken);

//            if (phoneResult == null)
//            {
//                return Result<EstablishmentPhone>.Failure(
//                    ErrorType.NotFound,
//                    new Error("NotFound", $"EstablishmentPhone with ID '{establishmentPhoneId}' not found."));
//            }
//            return Result<EstablishmentPhone>.Success(phoneResult);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentPhone>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<EstablishmentPhone>> AddAsync(
//        EstablishmentPhone establishmentPhone,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _ = await _establishmentPhoneDbContext.EstablishmentPhones.AddAsync(establishmentPhone, cancellationToken);
//            return Result<EstablishmentPhone>.Success(establishmentPhone);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentPhone>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<EstablishmentPhone>> UpdateAsync(
//        EstablishmentPhone establishmentPhone,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _ = _establishmentPhoneDbContext.EstablishmentPhones.Update(establishmentPhone);
//            return Result<EstablishmentPhone>.Success(establishmentPhone);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentPhone>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> DeleteAsync(
//        EstablishmentPhoneId id,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            EstablishmentPhone? phoneToDelete = await _establishmentPhoneDbContext.EstablishmentPhones
//                .FindAsync([id], cancellationToken);

//            if (phoneToDelete != null)
//            {
//                _ = _establishmentPhoneDbContext.EstablishmentPhones.Remove(phoneToDelete);
//                return Result.Success();
//            }
//            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"EstablishmentPhone with ID '{id}' not found."));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> ExistsAsync(
//        EstablishmentPhoneId id,
//        CancellationToken cancellationToken = default)
//        => await _establishmentPhoneDbContext.EstablishmentPhones.AnyAsync(e => e.Id == id, cancellationToken);

//    public Task DetachAsync(EstablishmentPhone establishmentPhone)
//    {
//        _establishmentPhoneDbContext.Entry(establishmentPhone).State = EntityState.Detached;
//        return Task.CompletedTask;
//    }

//    public async Task<Result> SetNonPrimaryForEstablishmentAsync(
//        EstablishmentId establishmentId,
//        EstablishmentPhoneId excludedPhoneId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            int affectedRows = await _establishmentPhoneDbContext.EstablishmentPhones
//                .Where(p => p.EstablishmentId == establishmentId && p.IsPrimary && p.Id != excludedPhoneId)
//                .ExecuteUpdateAsync(setters => setters
//                    .SetProperty(b => b.IsPrimary, false),
//                    cancellationToken);

//            return Result.Success();
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentPhone> spec)
//    {
//        try
//        {
//            IQueryable<EstablishmentPhone> query = SpecificationEvaluator<EstablishmentPhone>
//                .GetQuery(_establishmentPhoneDbContext.EstablishmentPhones, (BaseSpecification<EstablishmentPhone>)spec);
//            int count = await query.CountAsync();
//            return Result<int>.Success(count);
//        }
//        catch (Exception ex)
//        {
//            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<IEnumerable<EstablishmentPhone>>> ListAsync(ISpecification<EstablishmentPhone> spec)
//    {
//        try
//        {
//            IQueryable<EstablishmentPhone> query = _establishmentPhoneDbContext.EstablishmentPhones
//                .Include(x => x.EstablishmentPhoneNumber)
//                .AsQueryable();
//            var establishmentPhoneSpec = (BaseSpecification<EstablishmentPhone>)spec;
//            query = SpecificationEvaluator<EstablishmentPhone>.GetQuery(query, establishmentPhoneSpec);

//            List<EstablishmentPhone> establishmentPhones = await query.ToListAsync();
//            return Result<IEnumerable<EstablishmentPhone>>.Success(establishmentPhones);
//        }
//        catch (Exception ex)
//        {
//            return Result<IEnumerable<EstablishmentPhone>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> PhoneExistsWithSameNumberInEstablishment(
//        EstablishmentPhoneId excludedId,
//        string phoneNumber,
//        EstablishmentId establishmentId) => await _establishmentPhoneDbContext.EstablishmentPhones
//            .AnyAsync(ep => ep.EstablishmentId == establishmentId &&
//                           ep.EstablishmentPhoneNumber.PhoneNo == phoneNumber &&
//                           ep.Id != excludedId);
//}
