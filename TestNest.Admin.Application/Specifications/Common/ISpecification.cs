using System.Linq.Expressions;

namespace TestNest.Admin.Application.Specifications.Common;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> OrderBy { get; }
    List<(Expression<Func<T, object>> OrderByExpression, SortDirection Direction)> OrderByList { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
}
