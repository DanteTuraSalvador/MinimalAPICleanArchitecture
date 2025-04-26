using System.Linq.Expressions;
using TestNest.Admin.Application.Specifications.Common;

namespace TestNest.Admin.Application.Specifications.Extensions;

public static class SpecificationExtensions
{
    public static BaseSpecification<T> And<T>(this BaseSpecification<T> specification, ISpecification<T> other) where T : class
    {
        Expression<Func<T, bool>> combinedCriteria = specification.Criteria;

        if (other.Criteria != null)
        {
            if (combinedCriteria == null)
            {
                combinedCriteria = other.Criteria;
            }
            else
            {
                var originalCriteria = specification.Criteria;
                var otherCriteria = other.Criteria;

                var parameter = Expression.Parameter(typeof(T));
                var visitor = new ReplaceParameterVisitor(originalCriteria.Parameters[0], parameter);
                var left = visitor.Visit(originalCriteria.Body);
                visitor = new ReplaceParameterVisitor(otherCriteria.Parameters[0], parameter);
                var right = visitor.Visit(otherCriteria.Body);

                combinedCriteria = Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(left, right),
                    parameter);
            }
        }

        return new BaseSpecification<T>(combinedCriteria)
        {
            OrderBy = specification.OrderBy,
            OrderByList = specification.OrderByList,
            Take = specification.Take,
            Skip = specification.Skip,
            IsPagingEnabled = specification.IsPagingEnabled,
            Includes = specification.Includes
        };
    }

    private class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == _oldParameter)
            {
                return _newParameter;
            }
            return base.VisitParameter(node);
        }
    }
}