using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestNest.Admin.Application.Contracts.Interfaces.Persistence;
using TestNest.Admin.Application.Services;
using TestNest.Admin.Application.Specifications.Common;
using TestNest.Admin.Application.Specifications.EstablishmentSpecifications;
using TestNest.Admin.Domain.Establishments;
using TestNest.Admin.Infrastructure.Persistence.Common;
using TestNest.Admin.Infrastructure.Persistence.Repositories.Common;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions;
using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.StronglyTypeIds;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.Infrastructure.Persistence.Repositories;

public class EstablishmentRepository(ApplicationDbContext establishmentDbContext, ILogger<EstablishmentService> logger)
    : GenericRepository<Establishment, EstablishmentId>(establishmentDbContext), IEstablishmentRepository
{
    private readonly ApplicationDbContext _EstablishmentDbContext = establishmentDbContext;

    public override async Task<Result<Establishment>> GetByIdAsync(EstablishmentId establishmentId)
    {
        try
        {
            var spec = new EstablishmentSpecification(establishmentId);
            Establishment? establishment = await SpecificationEvaluator<Establishment>.GetQuery(_EstablishmentDbContext.Establishments, spec).FirstOrDefaultAsync();
            if (establishment == null)
            {
                var exception = EstablishmentException.NotFound();
                return Result<Establishment>.Failure(
                    ErrorType.NotFound,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }
            return Result<Establishment>.Success(establishment);
        }
        catch (Exception ex)
        {
            return Result<Establishment>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result<Establishment>> AddAsync(Establishment establishment)
    {
        try
        {
            _ = await _EstablishmentDbContext.Establishments.AddAsync(establishment);
            return Result<Establishment>.Success(establishment);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            var exception = EstablishmentException.DuplicateResource();
            return Result<Establishment>.Failure(ErrorType.Conflict,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }
        catch (Exception ex)
        {
            return Result<Establishment>.Failure(ErrorType.Internal,
                new Error("AddFailed", ex.Message));
        }
    }

    public override async Task<Result<Establishment>> UpdateAsync(Establishment establishment)
    {
        try
        {
            bool exists = await _EstablishmentDbContext.Establishments
                .AsNoTracking()
                .AnyAsync(x => x.Id == establishment.Id);

            if (!exists)
            {
                var exception = EstablishmentException.NotFound();
                return Result<Establishment>.Failure(ErrorType.NotFound,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }

            _ = _EstablishmentDbContext.Update(establishment);
            return Result<Establishment>.Success(establishment);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            var exception = EstablishmentException.DuplicateResource();
            return Result<Establishment>.Failure(ErrorType.Conflict,
                new Error(exception.Code.ToString(), exception.Message.ToString()));
        }
        catch (Exception ex)
        {
            return Result<Establishment>.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public override async Task<Result> DeleteAsync(EstablishmentId establishmentId)
    {
        try
        {
            Establishment? establishment = await _EstablishmentDbContext.Establishments.FindAsync(establishmentId);
            if (establishment == null)
            {
                var exception = EstablishmentException.NotFound();
                return Result.Failure(ErrorType.NotFound,
                    new Error(exception.Code.ToString(), exception.Message.ToString()));
            }
            _ = _EstablishmentDbContext.Establishments.Remove(establishment);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                ErrorType.Internal,
                new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<bool> ExistsAsync(EstablishmentId establishmentId)
        => await _EstablishmentDbContext.Establishments.AnyAsync(e => e.Id == establishmentId);

    public async Task DetachAsync(Establishment establishment)
    {
        _EstablishmentDbContext.Entry(establishment).State = EntityState.Detached;
        await Task.CompletedTask;
    }

    public async Task<bool> EstablishmentIdExists(EstablishmentId establishmentId)
        => await _EstablishmentDbContext.Establishments
            .AnyAsync(e => e.Id == establishmentId);

    public async Task<bool> ExistsWithNameAndEmailAsync(
        EstablishmentName establishmentName,
        EmailAddress emailAddress,
        EstablishmentId establishmentId)
        => await _EstablishmentDbContext.Establishments.AnyAsync(
            e => e.EstablishmentName.Name == establishmentName.Name &&
                 e.EstablishmentEmail.Email == emailAddress.Email &&
                 e.Id != establishmentId);

    public async Task<Result<int>> CountAsync(ISpecification<Establishment> spec)
    {
        try
        {
            IQueryable<Establishment> query = SpecificationEvaluator<Establishment>.GetQuery(
                _EstablishmentDbContext.Establishments,
                (BaseSpecification<Establishment>)spec);
            int count = await query.CountAsync();
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<IEnumerable<Establishment>>> ListAsync(ISpecification<Establishment> spec)
    {
        try
        {
            IQueryable<Establishment> query = _EstablishmentDbContext.Establishments
                .Include(x => x.EstablishmentEmail)
                .Include(x => x.EstablishmentName)
                .AsQueryable();
            var establishmentSpec = (BaseSpecification<Establishment>)spec;
            query = SpecificationEvaluator<Establishment>.GetQuery(query, establishmentSpec);
            List<Establishment> establishments = await query.ToListAsync();
            return Result<IEnumerable<Establishment>>.Success(establishments);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Establishment>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }
}

//public class EstablishmentRepository(ApplicationDbContext establishmentDbContext, ILogger<EstablishmentService> logger) : IEstablishmentRepository
//{
//    private readonly ApplicationDbContext _EstablishmentDbContext = establishmentDbContext;
//    private readonly ILogger<EstablishmentService> _logger = logger;

//    public async Task<Result<Establishment>> GetByIdAsync(EstablishmentId establishmentId)
//    {
//        try
//        {
//            var spec = new EstablishmentSpecification(establishmentId);
//            Establishment? establishment = await SpecificationEvaluator<Establishment>.GetQuery(_EstablishmentDbContext.Establishments, spec).FirstOrDefaultAsync();
//            if (establishment == null)
//            {
//                var exception = EstablishmentException.NotFound();
//                return Result<Establishment>.Failure(
//                    ErrorType.NotFound,
//                    new Error(exception.Code.ToString(), exception.Message.ToString()));
//            }

//            return Result<Establishment>.Success(establishment);
//        }
//        catch (Exception ex)
//        {
//            return Result<Establishment>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<Establishment>> AddAsync(Establishment establishment)
//    {
//        try
//        {
//            _ = await _EstablishmentDbContext.Establishments.AddAsync(establishment);
//            return Result<Establishment>.Success(establishment);
//        }
//        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
//        {
//            var exception = EstablishmentException.DuplicateResource();
//            return Result<Establishment>.Failure(ErrorType.Conflict,
//               new Error(exception.Code.ToString(), exception.Message.ToString()));
//        }
//        catch (Exception ex)
//        {
//            return Result<Establishment>.Failure(ErrorType.Internal,
//                new Error("AddFailed", ex.Message));
//        }
//    }

//    public async Task<Result<Establishment>> UpdateAsync(Establishment establishment)
//    {
//        try
//        {
//            bool exists = await _EstablishmentDbContext.Establishments
//                .AsNoTracking()
//                .AnyAsync(x => x.Id == establishment.Id);

//            if (!exists)
//            {
//                var exception = EstablishmentException.NotFound();
//                return Result<Establishment>.Failure(ErrorType.NotFound,
//                    new Error(exception.Code.ToString(), exception.Message.ToString()));
//            }

//            _ = _EstablishmentDbContext.Update(establishment);

//            return Result<Establishment>.Success(establishment);
//        }
//        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
//        {
//            var exception = EstablishmentException.DuplicateResource();
//            return Result<Establishment>.Failure(ErrorType.Conflict,
//               new Error(exception.Code.ToString(), exception.Message.ToString()));
//        }
//        catch (Exception ex)
//        {
//            return Result<Establishment>.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result> DeleteAsync(EstablishmentId establishmentId)
//    {
//        try
//        {
//            int rowsDeleted = await _EstablishmentDbContext.Establishments
//                .Where(p => p.Id == establishmentId)
//                .ExecuteDeleteAsync();

//            var exception = EstablishmentException.DuplicateResource();
//            return rowsDeleted > 0
//                ? Result.Success()
//                : Result.Failure(ErrorType.NotFound,
//                    new Error(exception.Code.ToString(), exception.Message.ToString()));
//        }
//        catch (Exception ex)
//        {
//            return Result.Failure(
//                ErrorType.Internal,
//                new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<bool> ExistsAsync(EstablishmentId establishmentId)
//        => await _EstablishmentDbContext.Establishments.AnyAsync(e => e.Id == establishmentId);

//    public async Task DetachAsync(Establishment establishment)
//    {
//        _EstablishmentDbContext.Entry(establishment).State = EntityState.Detached;
//        await Task.CompletedTask;
//    }

//    public async Task<bool> EstablishmentIdExists(EstablishmentId establishmentId)
//        => await _EstablishmentDbContext.Establishments
//            .AnyAsync(e => e.Id == establishmentId);

//    public async Task<bool> ExistsWithNameAndEmailAsync(
//        EstablishmentName establishmentName,
//        EmailAddress emailAddress,
//        EstablishmentId establishmentId)
//        => await _EstablishmentDbContext.Establishments.AnyAsync(
//            e => e.EstablishmentName.Name == establishmentName.Name &&
//                 e.EstablishmentEmail.Email == emailAddress.Email &&
//                 e.Id != establishmentId);

//    public async Task<Result<int>> CountAsync(ISpecification<Establishment> spec)
//    {
//        try
//        {
//            IQueryable<Establishment> query = SpecificationEvaluator<Establishment>.GetQuery(
//                _EstablishmentDbContext.Establishments,
//                (BaseSpecification<Establishment>)spec);
//            int count = await query.CountAsync();
//            return Result<int>.Success(count);
//        }
//        catch (Exception ex)
//        {
//            return Result<int>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }

//    public async Task<Result<IEnumerable<Establishment>>> ListAsync(ISpecification<Establishment> spec)
//    {
//        try
//        {
//            IQueryable<Establishment> query = _EstablishmentDbContext.Establishments
//                .Include(x => x.EstablishmentEmail)
//                .Include(x => x.EstablishmentName)
//                .AsQueryable();
//            var establishmentSpec = (BaseSpecification<Establishment>)spec;
//            query = SpecificationEvaluator<Establishment>.GetQuery(query, establishmentSpec);
//            List<Establishment> establishments = await query.ToListAsync();
//            return Result<IEnumerable<Establishment>>.Success(establishments);
//        }
//        catch (Exception ex)
//        {
//            return Result<IEnumerable<Establishment>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
//        }
//    }
//}
