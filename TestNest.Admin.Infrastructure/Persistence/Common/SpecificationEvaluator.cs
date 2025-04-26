using Microsoft.EntityFrameworkCore;
using TestNest.Admin.Application.Specifications.Common;

namespace TestNest.Admin.Infrastructure.Persistence.Common;

public static class SpecificationEvaluator<TEntity> where TEntity : class
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, BaseSpecification<TEntity> spec)
    {
        var query = inputQuery;

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        if (spec.OrderBy != null && spec.OrderBy.Any())
        {
            query = query.OrderBy(spec.OrderBy.First());
        }
        else if (spec.OrderByList != null && spec.OrderByList.Any())
        {
            query = spec.OrderByList.Aggregate((IQueryable<TEntity>)query, (current, order) =>
                order.Direction == SortDirection.Ascending
                    ? current.OrderBy(order.OrderByExpression)
                    : current.OrderByDescending(order.OrderByExpression));
        }

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        if (spec.IsPagingEnabled)
        {
            query = query.Skip(spec.Skip).Take(spec.Take);
        }

        return query;
    }
}
