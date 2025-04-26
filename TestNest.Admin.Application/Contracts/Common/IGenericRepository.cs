using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.Common.Results;
using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.Application.Contracts.Common;

public interface IGenericRepository<T, TId>
    where T : BaseEntity<TId>
    where TId : StronglyTypedId<TId>
{
    Task<Result<T>> GetByIdAsync(TId id);

    Task<Result<IEnumerable<T>>> GetAllAsync();

    Task<Result<T>> AddAsync(T entity);

    Task<Result<T>> UpdateAsync(T entity);

    Task<Result> DeleteAsync(TId id);
}
