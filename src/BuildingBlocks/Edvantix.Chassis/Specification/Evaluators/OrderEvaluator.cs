using Edvantix.Chassis.Specification.Expressions;

namespace Edvantix.Chassis.Specification.Evaluators;

public sealed class OrderEvaluator : IEvaluator
{
    private OrderEvaluator() { }

    public static OrderEvaluator Instance { get; } = new();

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification)
        where T : class
    {
        if (specification.OrderExpressions is null || !specification.OrderExpressions.Any())
        {
            return query;
        }

        if (
            specification.OrderExpressions.Count(x =>
                x.OrderType is OrderType.OrderBy or OrderType.OrderByDescending
            ) > 1
        )
        {
            throw new InvalidOperationException(
                "Only one OrderBy or OrderByDescending is allowed."
            );
        }

        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var orderExpression in specification.OrderExpressions)
        {
            orderedQuery = orderExpression.OrderType switch
            {
                OrderType.OrderBy => query.OrderBy(orderExpression.KeySelector),
                OrderType.OrderByDescending => query.OrderByDescending(orderExpression.KeySelector),
                OrderType.ThenBy => orderedQuery?.ThenBy(orderExpression.KeySelector) 
                                    ?? throw new InvalidOperationException("ThenBy requires a preceding OrderBy."),
                OrderType.ThenByDescending => orderedQuery?.ThenByDescending(orderExpression.KeySelector)
                                              ?? throw new InvalidOperationException("ThenByDescending requires a preceding OrderBy."),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(orderExpression.OrderType),
                    "Invalid order type."
                ),
            };

            // После первого OrderBy/OrderByDescending, query больше не используется
            if (orderExpression.OrderType is OrderType.OrderBy or OrderType.OrderByDescending)
            {
                query = orderedQuery;
            }
        }

        return orderedQuery ?? query;
    }
}
