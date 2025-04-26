using System.Linq.Expressions;

namespace TestNest.Admin.Application.Specifications.Common;

public class BaseSpecification<T> : ISpecification<T> where T : class
{
    public Expression<Func<T, bool>> Criteria { get; set; }
    public List<Expression<Func<T, object>>> OrderBy { get; set; } = [];
    public List<(Expression<Func<T, object>> OrderByExpression, SortDirection Direction)> OrderByList { get; set; } = [];
    public int Take { get; set; }
    public int Skip { get; set; }
    public bool IsPagingEnabled { get; set; }
    public List<Expression<Func<T, object>>> Includes { get; set; } = [];

    public List<string> IncludeStrings { get; set; } = []; 

    public BaseSpecification()
    {
    }

    public BaseSpecification(Expression<Func<T, bool>> criteria) => Criteria = criteria;

    protected void AddCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;

    public void AddOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy.Add(orderByExpression);

    public void AddOrderBy(Expression<Func<T, object>> orderByExpression, SortDirection direction) => OrderByList.Add((orderByExpression, direction));

    public void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);

    protected void AddInclude(string includeString) => IncludeStrings.Add(includeString);
}
