using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Contracts.Common;
using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.Exceptions.Common;

namespace TestNest.Admin.Infrastructure.Persistence.Repositories.Common;

public class GenericRepository<T, TId>(ApplicationDbContext dbContext) : IGenericRepository<T, TId>
    where T : BaseEntity<TId>
    where TId : StronglyTypedId<TId>
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public virtual async Task<Result<T>> GetByIdAsync(TId id)
    {
        try
        {
            T? entity = await _dbContext.Set<T>().FindAsync(id.Value);
            return entity != null ? Result<T>.Success(entity) : Result<T>.Failure(ErrorType.NotFound, new Error("NotFound", $"{typeof(T).Name} with ID '{id}' not found."));
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public virtual async Task<Result<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
            List<T> entities = await _dbContext.Set<T>().ToListAsync();
            return Result<IEnumerable<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<T>>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public virtual async Task<Result<T>> AddAsync(T entity)
    {
        try
        {
            _ = await _dbContext.Set<T>().AddAsync(entity);
            return Result<T>.Success(entity);
        }
        catch (DbUpdateException ex)
        {
            return Result<T>.Failure(ErrorType.Internal, new Error("DbUpdateError", ex.Message));
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public virtual async Task<Result<T>> UpdateAsync(T entity)
    {
        try
        {
            _ = _dbContext.Set<T>().Update(entity);
            return Result<T>.Success(entity);
        }
        catch (DbUpdateException ex)
        {
            return Result<T>.Failure(ErrorType.Internal, new Error("DbUpdateError", ex.Message));
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }

    public virtual async Task<Result> DeleteAsync(TId id)
    {
        try
        {
            T? entity = await _dbContext.Set<T>().FindAsync(id.Value);
            if (entity != null)
            {
                _ = _dbContext.Set<T>().Remove(entity);
                return Result.Success();
            }
            return Result.Failure(ErrorType.NotFound, new Error("NotFound", $"{typeof(T).Name} with ID '{id}' not found."));
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorType.Internal, new Error("DatabaseError", ex.Message));
        }
    }
}
