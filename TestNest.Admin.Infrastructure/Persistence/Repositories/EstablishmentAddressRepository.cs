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

public class EstablishmentAddressRepository(
    ApplicationDbContext establishmentAddressDbContext,
    ILogger<EstablishmentAddressRepository> logger)
    : GenericRepository<EstablishmentAddress, EstablishmentAddressId>(establishmentAddressDbContext), IEstablishmentAddressRepository
{
    private readonly ApplicationDbContext _establishmentAddressDbContext = establishmentAddressDbContext;
    private readonly ILogger<EstablishmentAddressRepository> _logger = logger;

    public override async Task<Result<EstablishmentAddress>> GetByIdAsync(EstablishmentAddressId id)
    {
        try
        {
            EstablishmentAddress? addressResult = await _establishmentAddressDbContext.EstablishmentAddresses
                .Include(ea => ea.Address)
                .FirstOrDefaultAsync(ea => ea.Id == id.Value);
            if (addressResult == null)
            {
                return Result<EstablishmentAddress>.Failure(
                    ErrorType.NotFound,
                    new Error("NotFound", $"EstablishmentAddress with ID '{id}' not found."));
            }
            return Result<EstablishmentAddress>.Success(addressResult);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentAddress>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<IEnumerable<EstablishmentAddress>>> GetAllAsync()
    {
        try
        {
            List<EstablishmentAddress> addresses = await _establishmentAddressDbContext.EstablishmentAddresses
                .Include(ea => ea.Address)
                .ToListAsync();
            return Result<IEnumerable<EstablishmentAddress>>.Success(addresses);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EstablishmentAddress>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<EstablishmentAddress>> AddAsync(EstablishmentAddress entity)
    {
        try
        {
            _ = await _establishmentAddressDbContext.EstablishmentAddresses.AddAsync(entity);
            return Result<EstablishmentAddress>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentAddress>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<EstablishmentAddress>> UpdateAsync(EstablishmentAddress entity)
    {
        try
        {
            _ = _establishmentAddressDbContext.EstablishmentAddresses.Update(entity);
            return Result<EstablishmentAddress>.Success(entity);
        }
        catch (Exception ex)
        {
            return Result<EstablishmentAddress>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result> DeleteAsync(EstablishmentAddressId id)
    {
        try
        {
            EstablishmentAddress? entityToDelete = await _establishmentAddressDbContext.EstablishmentAddresses
                .FindAsync(id.Value);
            if (entityToDelete != null)
            {
                _ = _establishmentAddressDbContext.EstablishmentAddresses.Remove(entityToDelete);
                return Result.Success();
            }
            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"{nameof(EstablishmentAddress)} with ID '{id}' not found."));
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> ExistsAsync(EstablishmentAddressId id)
        => await _establishmentAddressDbContext.EstablishmentAddresses.AnyAsync(e => e.Id == id);

    public Task DetachAsync(EstablishmentAddress establishmentAddress)
    {
        _establishmentAddressDbContext.Entry(establishmentAddress).State = EntityState.Detached;
        return Task.CompletedTask;
    }

    public async Task<Result> SetNonPrimaryForEstablishmentAsync(
        EstablishmentId establishmentId,
        EstablishmentAddressId excludedAddressId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            int affectedRows = await _establishmentAddressDbContext.EstablishmentAddresses
                .Where(ea => ea.EstablishmentId == establishmentId && ea.IsPrimary && ea.Id != excludedAddressId)
                .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.IsPrimary, false),
                    cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentAddress> spec)
    {
        try
        {
            IQueryable<EstablishmentAddress> query = SpecificationEvaluator<EstablishmentAddress>
                .GetQuery(_establishmentAddressDbContext.EstablishmentAddresses, (BaseSpecification<EstablishmentAddress>)spec);
            int count = await query.CountAsync();
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<IEnumerable<EstablishmentAddress>>> ListAsync(ISpecification<EstablishmentAddress> spec)
    {
        try
        {
            IQueryable<EstablishmentAddress> query = _establishmentAddressDbContext.EstablishmentAddresses
                .Include(x => x.Address)
                .AsQueryable();
            var establishmentAddressSpec = (BaseSpecification<EstablishmentAddress>)spec;
            query = SpecificationEvaluator<EstablishmentAddress>.GetQuery(query, establishmentAddressSpec);
            List<EstablishmentAddress> establishmentAddresses = await query.ToListAsync();
            return Result<IEnumerable<EstablishmentAddress>>.Success(establishmentAddresses);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EstablishmentAddress>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> AddressExistsWithSameCoordinatesInEstablishment(
        EstablishmentAddressId excludedId,
        decimal latitude,
        decimal longitude,
        EstablishmentId establishmentId)
    {
        try
        {
            return await _establishmentAddressDbContext.EstablishmentAddresses
                .Include(ea => ea.Address)
                .AnyAsync(ea => ea.Address.Latitude == latitude &&
                                 ea.Address.Longitude == longitude &&
                                 ea.EstablishmentId == establishmentId &&
                                 ea.Id != excludedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for existing address coordinates in establishment {EstablishmentId}", establishmentId);
            return false;
        }
    }
}

//public class EstablishmentAddressRepository(
//    ApplicationDbContext establishmentAddressDbContext,
//    ILogger<EstablishmentAddressRepository> logger) : IEstablishmentAddressRepository
//{
//    private readonly ApplicationDbContext establishmentAddressDbContext = establishmentAddressDbContext;
//    private readonly ILogger<EstablishmentAddressRepository> _logger = logger;

//    public async Task<Result<EstablishmentAddress>> GetByIdAsync(
//        EstablishmentAddressId establishmentAddressId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            EstablishmentAddress? addressResult = await establishmentAddressDbContext.EstablishmentAddresses
//                .FirstOrDefaultAsync(ea => ea.Id == establishmentAddressId, cancellationToken);

//            if (addressResult == null)
//            {
//                return Result<EstablishmentAddress>.Failure(
//                    ErrorType.NotFound,
//                    new Error("NotFound", $"EstablishmentAddress with ID '{establishmentAddressId}' not found."));
//            }
//            return Result<EstablishmentAddress>.Success(addressResult);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentAddress>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<EstablishmentAddress>> AddAsync(
//        EstablishmentAddress establishmentAddress,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _ = await establishmentAddressDbContext.EstablishmentAddresses.AddAsync(establishmentAddress, cancellationToken);
//            return Result<EstablishmentAddress>.Success(establishmentAddress);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentAddress>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<EstablishmentAddress>> UpdateAsync(
//        EstablishmentAddress establishmentAddress,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _ = establishmentAddressDbContext.EstablishmentAddresses.Update(establishmentAddress);
//            return Result<EstablishmentAddress>.Success(establishmentAddress);
//        }
//        catch (Exception ex)
//        {
//            return Result<EstablishmentAddress>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> DeleteAsync(
//        EstablishmentAddressId id,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            EstablishmentAddress? addressToDelete = await establishmentAddressDbContext.EstablishmentAddresses
//                .FindAsync([id], cancellationToken);

//            if (addressToDelete != null)
//            {
//                _ = establishmentAddressDbContext.EstablishmentAddresses.Remove(addressToDelete);
//                return Result.Success();
//            }
//            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"EstablishmentAddress with ID '{id}' not found."));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> ExistsAsync(
//        EstablishmentAddressId id,
//        CancellationToken cancellationToken = default)
//        => await establishmentAddressDbContext.EstablishmentAddresses.AnyAsync(e => e.Id == id, cancellationToken);

//    public Task DetachAsync(EstablishmentAddress establishmentAddress)
//    {
//        establishmentAddressDbContext.Entry(establishmentAddress).State = EntityState.Detached;
//        return Task.CompletedTask;
//    }

//    public async Task<Result> SetNonPrimaryForEstablishmentAsync(
//        EstablishmentId establishmentId,
//        EstablishmentAddressId excludedAddressId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            int affectedRows = await establishmentAddressDbContext.EstablishmentAddresses
//                .Where(ea => ea.EstablishmentId == establishmentId && ea.IsPrimary && ea.Id != excludedAddressId)
//                .ExecuteUpdateAsync(setters => setters
//                .SetProperty(b => b.IsPrimary, false),
//                cancellationToken);

//            return Result.Success();
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<int>> CountAsync(ISpecification<EstablishmentAddress> spec)
//    {
//        try
//        {
//            IQueryable<EstablishmentAddress> query = SpecificationEvaluator<EstablishmentAddress>
//                .GetQuery(establishmentAddressDbContext.EstablishmentAddresses, (BaseSpecification<EstablishmentAddress>)spec);
//            int count = await query.CountAsync();
//            return Result<int>.Success(count);
//        }
//        catch (Exception ex)
//        {
//            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<IEnumerable<EstablishmentAddress>>> ListAsync(ISpecification<EstablishmentAddress> spec)
//    {
//        try
//        {
//            IQueryable<EstablishmentAddress> query = establishmentAddressDbContext.EstablishmentAddresses
//                .Include(x => x.Address)
//                .AsQueryable();
//            var establishmentAddressSpec = (BaseSpecification<EstablishmentAddress>)spec;
//            query = SpecificationEvaluator<EstablishmentAddress>.GetQuery(query, establishmentAddressSpec);

//            List<EstablishmentAddress> establishmentAddresses = await query.ToListAsync();
//            return Result<IEnumerable<EstablishmentAddress>>.Success(establishmentAddresses);
//        }
//        catch (Exception ex)
//        {
//            return Result<IEnumerable<EstablishmentAddress>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> AddressExistsWithSameCoordinatesInEstablishment(
//         EstablishmentAddressId excludedId,
//         decimal latitude,
//         decimal longitude,
//         EstablishmentId establishmentId) => await establishmentAddressDbContext.EstablishmentAddresses
//            .AnyAsync(ea => ea.Address.Latitude == latitude &&
//                           ea.Address.Longitude == longitude &&
//                           ea.EstablishmentId == establishmentId &&
//                           ea.Id != excludedId);
//}
