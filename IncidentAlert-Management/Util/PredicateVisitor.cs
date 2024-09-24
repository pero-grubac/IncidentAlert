using System.Linq.Expressions;

namespace IncidentAlert_Management.Util
{
    public class PredicateVisitor<TSource, TTarget>(ParameterExpression parameter) : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter = parameter;

        public override Expression Visit(Expression node)
        {
            // Customize this method to handle specific expressions
            return base.Visit(node);
        }

        public Expression<Func<TTarget, bool>> Visit(Expression<Func<TSource, bool>> source)
        {
            var body = Visit(source.Body);
            return Expression.Lambda<Func<TTarget, bool>>(body, _parameter);
        }
    }
}
